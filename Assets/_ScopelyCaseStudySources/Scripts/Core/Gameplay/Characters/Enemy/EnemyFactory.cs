using System.Threading;
using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public static class EnemyFactory
    {
        public static async UniTask<Enemy> CreateEnemy(GameSession gameSession, EnemyView enemyView, EnemyConfig enemyConfig, CancellationToken cancellationToken)
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

            enemy.SetSession(gameSession);
            await enemy.InitializeController(enemyData, enemyView, cancellationToken);

            return enemy;
        }
    }
}
