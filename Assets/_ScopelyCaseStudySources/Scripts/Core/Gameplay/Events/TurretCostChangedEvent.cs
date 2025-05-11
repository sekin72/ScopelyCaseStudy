using CerberusFramework.Core.Managers.Pool;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct TurretCostChangedEvent
    {
        public readonly PoolKeys PoolKey;
        public readonly int Cost;

        public TurretCostChangedEvent(PoolKeys poolKey, int cost)
        {
            PoolKey = poolKey;
            Cost = cost;
        }
    }
}
