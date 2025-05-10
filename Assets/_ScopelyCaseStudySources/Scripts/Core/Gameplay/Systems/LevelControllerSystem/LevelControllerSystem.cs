using System;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using CFGameClient.Core.Gameplay.Systems.ViewSpawner;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.LevelAssets;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.LevelControllerSystem
{
    [CreateAssetMenu(fileName = "LevelControllerSystem", menuName = "ScopelyCaseStudy/Systems/LevelControllerSystem", order = 3)]
    public class LevelControllerSystem : GameSystem, ILevelControllerSystem
    {
        public override Type RegisterType => typeof(ILevelControllerSystem);
        private const int LevelPoolIndexStart = 3000;

        public LevelView Level { get; private set; }

        private IViewSpawnerSystem _viewSpawnerSystem;
        private LevelControllerSystemView _view;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _viewSpawnerSystem = Session.GetSystem<IViewSpawnerSystem>();
            _view = _viewSpawnerSystem.Spawn<LevelControllerSystemView>(PoolKeys.LevelControllerSystemView);
            await _view.Initialize(cancellationToken);

            Level = _viewSpawnerSystem.Spawn<LevelView>((PoolKeys)(LevelPoolIndexStart+ Session.GameSessionSaveStorage.CurrentLevel));
            _view.AttachLevel(Level);
            await Level.Initialize(cancellationToken);
        }

        public override void Activate()
        {
            _view.Activate();
            Level.Activate();
        }

        public override void Deactivate()
        {
            _view.Deactivate();
            Level.Deactivate();
        }

        public override void Dispose()
        {
            _view.Dispose();
            _viewSpawnerSystem.Despawn(PoolKeys.LevelControllerSystemView, _view);

            Level.Dispose();
            _viewSpawnerSystem.Despawn((PoolKeys)(LevelPoolIndexStart + Session.GameSessionSaveStorage.CurrentLevel), Level);
        }
    }
}
