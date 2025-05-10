using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct DetachParticleEvent
    {
        public readonly PoolKeys PoolKey;
        public readonly GameObject ParticleGameObject;

        public DetachParticleEvent(PoolKeys poolKey, GameObject particleObject)
        {
            PoolKey = poolKey;
            ParticleGameObject = particleObject;
        }
    }
}