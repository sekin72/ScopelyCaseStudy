using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct AttachParticleEvent
    {
        public readonly PoolKeys PoolKey;
        public readonly GameObject ParticleGameObject;

        public AttachParticleEvent(PoolKeys poolKey, GameObject particleObject)
        {
            PoolKey = poolKey;
            ParticleGameObject = particleObject;
        }
    }
}