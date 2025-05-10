using CFGameClient.Core.Gameplay.Systems;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem
{
    public interface IEnemyControllerSystem : IGameSystem
    {
        Enemy GetEnemyFromView(EnemyView enemyView);
    }
}
