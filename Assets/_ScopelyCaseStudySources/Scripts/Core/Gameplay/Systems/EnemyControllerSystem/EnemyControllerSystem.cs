using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Utilities.Extensions;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Characters;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemySpawnerSystem;
using UnityEngine;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem
{
    [CreateAssetMenu(fileName = "EnemyControllerSystem", menuName = "ScopelyCaseStudy/Systems/EnemyControllerSystem", order = 1)]
    public class EnemyControllerSystem : GameSystem, IEnemyControllerSystem, ILateTickable
    {
        public override Type RegisterType => typeof(IEnemyControllerSystem);

        private IEnemySpawnerSystem _enemySpawnerSystem;
        private IPublisher<WaveStartedEvent> _waveStartedEventPublisher;

        private List<Enemy> _enemies;
        private Dictionary<EnemyView, Enemy> _enemyViewToEnemyMap;

        private int _currentWaveEnemyCount;
        private int _currentWave;
        private int _maxWave;

        private IDisposable _messageSubscription;

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            _enemySpawnerSystem = Session.GetSystem<IEnemySpawnerSystem>();

            _enemies = new List<Enemy>();
            _enemyViewToEnemyMap = new Dictionary<EnemyView, Enemy>();
            _currentWave = Session.GameSessionSaveStorage.WaveIndex;
            _maxWave = Session.LevelData.WaveData.Count;

            _currentWaveEnemyCount = 0;

            _waveStartedEventPublisher = GlobalMessagePipe.GetPublisher<WaveStartedEvent>();

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<EnemyKilledEvent>().Subscribe(OnEnemyDied).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();
        }

        public override void Activate()
        {
            LoadEnemies().Forget();
        }

        public override void Deactivate()
        {
            _messageSubscription?.Dispose();
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
            if (++_currentWave >= _maxWave)
            {
                Session.LevelFinished(true);
            }
            else
            {
                LoadEnemies().Forget();
            }
        }

        private async UniTask LoadEnemies()
        {
            _waveStartedEventPublisher.Publish(new WaveStartedEvent(_currentWave));

            var waveData = Session.LevelData.WaveData[_currentWave];
            var enemyLists = waveData.EnemyLists;

            Session.GameSessionSaveStorage.WaveIndex = _currentWave;
            Session.SaveGameSessionStorage();

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
                var enemyConfigCountPair = spawnList[i];
                Enemy enemy = null;
                tasks.Add(_enemySpawnerSystem.SpawnEnemyInRandomSpawnPoint(enemyConfigCountPair).ContinueWith(retrievedEnemy =>
                {
                    enemy = retrievedEnemy;

                    _enemies.Add(enemy);
                    _enemyViewToEnemyMap.Add(enemy.View, enemy);
                    enemy.ActivateController();
                }));

                await UniTask.Delay(TimeSpan.FromSeconds(waveData.SpawnInterval), cancellationToken: CancellationTokenSource.Token);
            }

            await UniTask.WhenAll(tasks);
        }

        private void OnEnemyDied(EnemyKilledEvent evt)
        {
            var enemy = evt.Enemy;
            if (!_enemies.Contains(enemy))
            {
                return;
            }

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

        public void LateTick()
        {
            foreach (var enemy in _enemies)
            {
                if (enemy.IsAlive())
                {
                    enemy.LateTick();
                }
            }
        }
    }
}
