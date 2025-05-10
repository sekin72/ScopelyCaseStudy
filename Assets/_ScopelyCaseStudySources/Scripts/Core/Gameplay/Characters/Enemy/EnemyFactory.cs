using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    public static class EnemyFactory
    {
        public static async UniTask<Enemy> CreateEnemy(EnemyView enemyView, EnemyConfig enemyConfig, CancellationToken cancellationToken)
        {
            Enemy enemy = null;
            EnemyData enemyData = new EnemyData(enemyConfig);

            switch (enemyConfig.EnemyType)
            {
                case EnemyTypes.SmallCreep:
                    enemy = new SmallCreep();
                    break;
                case EnemyTypes.BigCreep:
                    enemy = new BigCreep();
                    break;
                default:
                    break;
            }

            await enemy.InitializeController(enemyData, enemyView, cancellationToken);

            return enemy;
        }
    }
}
