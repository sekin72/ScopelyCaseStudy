using System.Threading;
using CFGameClient.Core.Gameplay.Systems;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemySpawnerSystem
{
    public interface IEnemySpawnerSystem : IGameSystem
    {
        public UniTask<Enemy> SpawnEnemyInRandomSpawnPoint(EnemyConfig enemyConfig, CancellationToken cancellationToken);
        public UniTask<Enemy> SpawnEnemyInIndexedSpawnPoint(EnemyConfig enemyConfig, int index, CancellationToken cancellationToken);
        public void DespawnEnemy(Enemy enemy);
    }
}
