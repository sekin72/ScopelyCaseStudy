using ScopelyCaseStudy.Core.Gameplay.Characters;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct EnemyKilledEvent
    {
        public readonly Enemy Enemy;
        public readonly int IncreaseScoreAmount;
        public readonly int IncreaseGoldAmount;

        public EnemyKilledEvent(Enemy enemy, int increaseScoreAmount, int increaseGoldAmount)
        {
            Enemy = enemy;
            IncreaseScoreAmount = increaseScoreAmount;
            IncreaseGoldAmount = increaseGoldAmount;
        }
    }
}
