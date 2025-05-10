using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using CFGameClient.Core.Gameplay.Systems.ViewSpawner;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;
using ScopelyCaseStudy.Core.Gameplay.Systems.LevelControllerSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemySpawnerSystem
{
    [CreateAssetMenu(fileName = "EnemySpawnerSystem", menuName = "ScopelyCaseStudy/Systems/EnemySpawnerSystem", order = 1)]
    public sealed class EnemySpawnerSystem : GameSystem, IEnemySpawnerSystem
    {
        public override Type RegisterType => typeof(IEnemySpawnerSystem);

        private IViewSpawnerSystem _viewSpawnerSystem;

        private List<Transform> _spawnPoints;

        private Transform _parentTransform;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _viewSpawnerSystem = Session.GetSystem<IViewSpawnerSystem>();

            _spawnPoints = new List<Transform>();

            var level = Session.GetSystem<ILevelControllerSystem>().Level;
            _parentTransform = level.EnemyParent;
            foreach (var spawnPoint in level.SpawnPoints)
            {
                _spawnPoints.Add(spawnPoint.transform);
            }
        }

        public override void Activate()
        {
            for(int i = 0; i < 5; i++)
            {
                SpawnEnemyInRandomSpawnPoint(new EnemyConfig()
                {
                    PoolKey = PoolKeys.SmallCreep,
                    EnemyType = EnemyTypes.SmallCreep
                }, CancellationToken.None).ContinueWith(enemy =>enemy.ActivateController()).Forget();

                SpawnEnemyInRandomSpawnPoint(new EnemyConfig()
                {
                    PoolKey = PoolKeys.BigCreep,
                    EnemyType = EnemyTypes.BigCreep
                }, CancellationToken.None).ContinueWith(enemy => enemy.ActivateController()).Forget();
            }
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
        }

        public UniTask<Enemy> SpawnEnemyInRandomSpawnPoint(EnemyConfig enemyConfig, CancellationToken cancellationToken)
        {
            return SpawnEnemyInIndexedSpawnPoint(enemyConfig, Random.Range(0, _spawnPoints.Count), cancellationToken);
        }

        public async UniTask<Enemy> SpawnEnemyInIndexedSpawnPoint(EnemyConfig enemyConfig, int index, CancellationToken cancellationToken)
        {
            var enemyView = _viewSpawnerSystem.Spawn<EnemyView>(enemyConfig.PoolKey);
            var enemy = await EnemyFactory.CreateEnemy(enemyView, enemyConfig, cancellationToken);

            enemyView.transform.SetParent(_parentTransform);

            enemyView.transform.position = _spawnPoints[index].position;

            return enemy;
        }

        public void DespawnEnemy(Enemy enemy)
        {
            _viewSpawnerSystem.Despawn(enemy.PoolKey, enemy.View);
        }
    }
}
