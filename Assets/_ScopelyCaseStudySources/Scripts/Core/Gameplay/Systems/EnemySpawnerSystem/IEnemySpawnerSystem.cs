using System.Threading;
using CFGameClient.Core.Gameplay.Systems;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemySpawnerSystem
{
    public interface IEnemySpawnerSystem : IGameSystem
    {
        public UniTask<Enemy> SpawnEnemyInRandomSpawnPoint(EnemyConfig enemyConfig);
        public UniTask<Enemy> SpawnEnemyInIndexedSpawnPoint(EnemyConfig enemyConfig, int index);
        public void DespawnEnemy(Enemy enemy);
    }
}
