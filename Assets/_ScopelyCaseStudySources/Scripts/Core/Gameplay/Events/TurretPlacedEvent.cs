using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct TurretPlacedEvent
    {
        public readonly PoolKeys PoolKey;
        public readonly Vector3 Position;

        public TurretPlacedEvent(PoolKeys poolKey, Vector3 position)
        {
            PoolKey = poolKey;
            Position = position;
        }
    }
}
