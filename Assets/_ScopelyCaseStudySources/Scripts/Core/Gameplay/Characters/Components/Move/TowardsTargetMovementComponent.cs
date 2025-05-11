using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public class TowardsTargetMovementComponent : MovementComponent
    {
        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public override void LateTick()
        {
            if (_target == null)
            {
                return;
            }

            var direction = (_target.position - transform.position).normalized;
            transform.position += direction * MoveSpeed * Time.deltaTime;
            _modelTransform.LookAt(_target.position);
        }
    }
}
