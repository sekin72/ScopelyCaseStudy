using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Utilities.Extensions;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemySpawnerSystem;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem
{
    [CreateAssetMenu(fileName = "EnemyControllerSystem", menuName = "ScopelyCaseStudy/Systems/EnemyControllerSystem", order = 1)]
    public class EnemyControllerSystem : GameSystem, IEnemyControllerSystem
    {
        public override Type RegisterType => typeof(IEnemyControllerSystem);

        private IEnemySpawnerSystem _enemySpawnerSystem;

        private List<Enemy> _enemies;
        private Dictionary<EnemyView, Enemy> _enemyViewToEnemyMap;

        private int _currentWaveEnemyCount;
        private int _currentWave;
        private int _maxWave;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _enemySpawnerSystem = Session.GetSystem<IEnemySpawnerSystem>();

            _enemies = new List<Enemy>();
            _enemyViewToEnemyMap = new Dictionary<EnemyView, Enemy>();
            _currentWave = 0;
            _maxWave = Session.LevelData.WaveData.Count;

            _currentWaveEnemyCount = 0;
        }

        public override void Activate()
        {
            LoadEnemies().Forget();
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
            foreach (var enemy in _enemies)
            {
                enemy.DeactivateController();
                enemy.DisposeController();
                _enemySpawnerSystem.DespawnEnemy(enemy);
            }
        }

        private void OnWaveFinished()
        {
            if (_currentWave >= _maxWave)
            {
                Session.LevelFinished(true);
            }
            else
            {
                LoadEnemies();
            }
        }

        private async UniTask LoadEnemies()
        {
            var waveData = Session.LevelData.WaveData[_currentWave++];
            var enemyLists = waveData.EnemyLists;

            var spawnList = new List<EnemyConfig>();

            for (int i = 0; i < enemyLists.Count; i++)
            {
                _currentWaveEnemyCount += enemyLists[i].Count;
                for (int j = 0; j < enemyLists[i].Count; j++)
                {
                    spawnList.Add(enemyLists[i].EnemyConfigs);
                }
            }

            spawnList.Shuffle();

            await UniTask.Delay(TimeSpan.FromSeconds(waveData.SpawnDelay), cancellationToken: CancellationTokenSource.Token);

            var tasks = new List<UniTask>();
            for (int i = 0; i < spawnList.Count; i++)
            {
                var enemySettingCountPair = spawnList[i];
                Enemy enemy = null;
                tasks.Add(_enemySpawnerSystem.SpawnEnemyInRandomSpawnPoint(enemySettingCountPair).ContinueWith(retrievedEnemy =>
                {
                    enemy = retrievedEnemy;
                    enemy.EnemyDiedEvent += OnEnemyDied;

                    _enemies.Add(enemy);
                    _enemyViewToEnemyMap.Add(enemy.View, enemy);
                    enemy.ActivateController();
                }));

                await UniTask.Delay(TimeSpan.FromSeconds(waveData.SpawnInterval), cancellationToken: CancellationTokenSource.Token);
            }

            await UniTask.WhenAll(tasks);
        }

        private void OnEnemyDied(Enemy enemy)
        {
            enemy.EnemyDiedEvent -= OnEnemyDied;

            _enemies.Remove(enemy);
            _enemyViewToEnemyMap.Remove(enemy.View);
            enemy.DisposeController();
            _enemySpawnerSystem.DespawnEnemy(enemy);

            if (--_currentWaveEnemyCount == 0)
            {
                OnWaveFinished();
            }
        }

        public Enemy GetEnemyFromView(EnemyView enemyView)
        {
            if (_enemyViewToEnemyMap.TryGetValue(enemyView, out var enemy))
            {
                return enemy;
            }

            return null;
        }
    }
}
