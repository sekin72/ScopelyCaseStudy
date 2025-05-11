using System;
using CerberusFramework.Core.Managers.Asset;
using CerberusFramework.Core.UI.Components;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.Systems.TurretPlacerSystem;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.UI
{
    public class LevelScenePanel : MonoBehaviour
    {
        [SerializeField] private CFText _scoreText;
        [SerializeField] private CFText _goldText;

        [SerializeField] private TurretSeller _regularTurret;
        [SerializeField] private TurretSeller _freezeTurret;

        private IDisposable _messageSubscription;

        private AddressableManager _addressableManager;
        private ITurretPlacerSystem _turretPlacerSystem;

        [Inject]
        public void Inject(AddressableManager addressableManager)
        {
            _addressableManager = addressableManager;
        }

        public void Initialize(GameSession gameSession, Camera camera)
        {
            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<ScoreChangedEvent>().Subscribe(OnScoreChanged).AddTo(bagBuilder);
            GlobalMessagePipe.GetSubscriber<GoldChangedEvent>().Subscribe(OnGoldChangedEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _turretPlacerSystem = gameSession.GetSystem<ITurretPlacerSystem>();

            _regularTurret.Initialize(camera, _turretPlacerSystem.TurretConfigs[0].Cost, _turretPlacerSystem.TurretConfigs[0].PoolKey);
            _freezeTurret.Initialize(camera, _turretPlacerSystem.TurretConfigs[1].Cost, _turretPlacerSystem.TurretConfigs[1].PoolKey);

            _scoreText.Text = gameSession.GameSessionSaveStorage.CurrentScore.ToString();
            _goldText.Text = gameSession.GameSessionSaveStorage.Gold.ToString();
        }

        public void Dispose()
        {
            _messageSubscription?.Dispose();

            _regularTurret.Dispose();
            _freezeTurret.Dispose();
        }

        private void OnScoreChanged(ScoreChangedEvent evt)
        {
            _scoreText.Text = evt.Score.ToString();
        }

        private void OnGoldChangedEvent(GoldChangedEvent evt)
        {
            _goldText.Text = evt.Gold.ToString();
        }
    }
}
