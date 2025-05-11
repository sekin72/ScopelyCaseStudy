using System;
using System.Threading;
using CerberusFramework.Utilities.MonoBehaviourUtilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    [RequireComponent(typeof(CollisionDetector))]
    public class TurretView : CharacterView
    {
        public event Action<EnemyView> EnemyEnteredReach;
        public event Action<EnemyView> EnemyExitedReach;

        [SerializeField] private Transform _body;
        [SerializeField] private CollisionDetector _collisionDetector;
        [SerializeField] private SphereCollider _sphereCollider;

        private Tween _rotateTween;

        private TurretData _turretData;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            _turretData = Data as TurretData;
            transform.position = _turretData.Position;

            _sphereCollider.radius = 5;

            transform.localScale = Vector3.one * 2.5f;
        }

        public override void Activate()
        {
            _collisionDetector.TriggerEntered += OnTriggerEntered;
            _collisionDetector.TriggerExited += OnTriggerExited;
        }

        public override void Deactivate()
        {
            _rotateTween?.Kill();
            _collisionDetector.TriggerEntered -= OnTriggerEntered;
            _collisionDetector.TriggerExited -= OnTriggerExited;
        }

        private void OnTriggerEntered(Collider other)
        {
            if (other.TryGetComponent(out EnemyView enemy))
            {
                EnemyEnteredReach?.Invoke(enemy);
            }
        }

        private void OnTriggerExited(Collider other)
        {
            if (other.TryGetComponent(out EnemyView enemy))
            {
                EnemyExitedReach?.Invoke(enemy);
            }
        }

        public void TurnToClosestEnemy(EnemyView enemyView)
        {
            _rotateTween?.Kill();
            _rotateTween = _body.DOLookAt(enemyView.transform.position, 0.1f);
        }
    }
}
