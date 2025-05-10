using System.Collections.Generic;
using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace CFGameClient.Core.Gameplay.Systems.ExternalParticles
{
    public class ExternalParticlesSystemView : MonoBehaviour
    {
        private readonly Dictionary<PoolKeys, List<GameObject>> _particles = new Dictionary<PoolKeys, List<GameObject>>();

        public void AttachGameObject(PoolKeys key, GameObject particleGameObject)
        {
            if (!_particles.ContainsKey(key))
            {
                _particles.Add(key, new List<GameObject>());
            }

            _particles[key].Add(particleGameObject);
            particleGameObject.transform.SetParent(transform);
        }
    }
}