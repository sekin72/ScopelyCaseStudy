using System;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public class TurretColliderProxy : MonoBehaviour
    {
        public event Action<EnemyView> OnEnemyCollided;
        public event Action<EnemyView> OnEnemyExited;

        [SerializeField] private SphereCollider _sphereCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out EnemyView enemy))
            {
                OnEnemyCollided?.Invoke(enemy);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out EnemyView enemy))
            {
                OnEnemyExited?.Invoke(enemy);
            }
        }

        public void SetColliderRadius(float radius)
        {
            _sphereCollider.radius = Math.Max(radius, 1);
        }
    }
}
