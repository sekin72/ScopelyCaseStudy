using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using CFGameClient.Core.Gameplay.Systems.ViewSpawner;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Characters.Turrets;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.Systems.LevelControllerSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.TurretPlacerSystem
{
    [CreateAssetMenu(fileName = "TurretPlacerSystem", menuName = "ScopelyCaseStudy/Systems/TurretPlacerSystem", order = 3)]
    public class TurretPlacerSystem : GameSystem, ITurretPlacerSystem, ILateTickable
    {
        public override Type RegisterType => typeof(ITurretPlacerSystem);

        [field: SerializeField] public List<TurretConfig> TurretConfigs { get; private set; }

        private IViewSpawnerSystem _viewSpawnerSystem;

        private List<Turret> _turrets = new List<Turret>();

        private IDisposable _messageSubscription;
        private IPublisher<TurretSoldEvent> _turretSoldEventPublisher;
        private IPublisher<FirstInputTakenEvent> _firstInputTakenEventPublisher;
        private IPublisher<TurretCostChangedEvent> _turretCostChangedEventPublisher;
        private IObjectResolver _objectResolver;

        private bool _firstInputTaken;
        private Dictionary<PoolKeys, int> _turretCostDictionary = new Dictionary<PoolKeys, int>();

        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Inject(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public override async UniTask Initialize(GameSession gameSessionBase, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSessionBase, cancellationToken);

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _firstInputTaken = false;

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<TurretPlacedEvent>().Subscribe(OnSellerPlacedEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _turretSoldEventPublisher = GlobalMessagePipe.GetPublisher<TurretSoldEvent>();
            _firstInputTakenEventPublisher = GlobalMessagePipe.GetPublisher<FirstInputTakenEvent>();
            _turretCostChangedEventPublisher = GlobalMessagePipe.GetPublisher<TurretCostChangedEvent>();

            _turrets.Clear();
            _turretCostDictionary.Clear();

            for (var i = 0; i < TurretConfigs.Count; i++)
            {
                _turretCostDictionary.Add(TurretConfigs[i].PoolKey, TurretConfigs[i].Cost);
            }

            _viewSpawnerSystem = Session.GetSystem<IViewSpawnerSystem>();

            var placedTurretPositions = Session.GameSessionSaveStorage.PlacedTurretPositions;
            var placedTurretPoolKeys = Session.GameSessionSaveStorage.PlacedTurretPoolKeys;

            var tasksList = new List<UniTask>();
            for (var i = 0; i < placedTurretPositions.Count; i++)
            {
                var position = placedTurretPositions[i];
                var poolKeyID = placedTurretPoolKeys[i];
                var turretConfig = TurretConfigs.FirstOrDefault(turret => turret.PoolKey == poolKeyID);

                tasksList.Add(CreateTurret(turretConfig, position));
            }

            await UniTask.WhenAll(tasksList);
        }

        public override void Activate()
        {
            if (!_firstInputTaken && Session.GameSessionSaveStorage.PlacedTurretPositions.Count > 0)
            {
                _firstInputTakenEventPublisher.Publish(new FirstInputTakenEvent());
                _firstInputTaken = true;
            }

            for (var i = 0; i < TurretConfigs.Count; i++)
            {
                _turretCostChangedEventPublisher.Publish(new TurretCostChangedEvent(TurretConfigs[i].PoolKey, _turretCostDictionary[TurretConfigs[i].PoolKey]));
            }
        }

        public override void Deactivate()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            foreach (var turret in _turrets)
            {
                turret.DeactivateController();
            }
        }

        public override void Dispose()
        {
            _messageSubscription?.Dispose();
            foreach (var turret in _turrets)
            {
                turret.DisposeController();
                _viewSpawnerSystem.Despawn(turret.Data.TurretConfig.PoolKey, turret.View);
            }
        }

        private void OnSellerPlacedEvent(TurretPlacedEvent evt)
        {
            var poolKey = evt.PoolKey;
            var cost = _turretCostDictionary[poolKey];
            var turretConfig = TurretConfigs.FirstOrDefault(turret => turret.PoolKey == poolKey);

            CreateTurret(turretConfig, evt.Position).Forget();

            _turretSoldEventPublisher.Publish(new TurretSoldEvent(-cost));

            Session.GameSessionSaveStorage.PlacedTurretPoolKeys.Add(turretConfig.PoolKey);
            Session.GameSessionSaveStorage.PlacedTurretPositions.Add(evt.Position);

            Session.SaveGameSessionStorage();

            if (!_firstInputTaken)
            {
                _firstInputTakenEventPublisher.Publish(new FirstInputTakenEvent());
                _firstInputTaken = true;
            }
        }

        private async UniTask CreateTurret(TurretConfig turretConfig, Vector3 position)
        {
            var turretData = new TurretData(turretConfig, Vector3.zero);
            var turretView = _viewSpawnerSystem.Spawn<TurretView>(turretConfig.PoolKey);
            var turret = new Turret();
            _turrets.Add(turret);

            turret.SetSession(Session);
            await turret.InitializeController(turretData, turretView, _cancellationTokenSource.Token);
            _objectResolver.InjectGameObject(turretView.gameObject);

            turret.ActivateController();

            turretView.transform.SetParent(Session.GetSystem<ILevelControllerSystem>().Level.transform);
            turretView.transform.position = position;

            _turretCostDictionary[turretConfig.PoolKey] = (int)(_turretCostDictionary[turretConfig.PoolKey] * turretConfig.CostMultiplier);
            _turretCostChangedEventPublisher.Publish(new TurretCostChangedEvent(turretConfig.PoolKey, _turretCostDictionary[turretConfig.PoolKey]));
        }

        public void LateTick()
        {
            foreach (var turret in _turrets)
            {
                turret.LateTick();
            }
        }
    }
}
