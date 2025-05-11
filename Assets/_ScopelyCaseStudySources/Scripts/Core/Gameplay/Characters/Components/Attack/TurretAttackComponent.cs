using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public class TurretAttackComponent : AttackComponent
    {
        [SerializeField] private TurretColliderProxy _colliderProxy;
        private IEnemyControllerSystem _enemyControllerSystem;
        private List<ICharacter> _targetsInRange;

        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);

            _enemyControllerSystem = GameSession.GetSystem<IEnemyControllerSystem>();

            _targetsInRange = new List<ICharacter>();

            _colliderProxy.OnEnemyCollided += OnEnemyCollided;
            _colliderProxy.OnEnemyExited += OnEnemyExited;
            _colliderProxy.SetColliderRadius(AttackRange);

            Attack();
        }

        public override void Dispose()
        {
            _colliderProxy.OnEnemyCollided -= OnEnemyCollided;
            _colliderProxy.OnEnemyExited -= OnEnemyExited;

            base.Dispose();
        }

        private void OnEnemyCollided(EnemyView enemyView)
        {
            var enemy = _enemyControllerSystem.GetEnemyFromView(enemyView);
            _targetsInRange.Add(enemy);
            enemy.EnemyDiedEvent += OnEnemyDied;
        }

        private void OnEnemyExited(EnemyView enemyView)
        {
            var enemy = _enemyControllerSystem.GetEnemyFromView(enemyView);
            enemy.EnemyDiedEvent -= OnEnemyDied;

            if (_targetsInRange.Contains(enemy))
            {
                _targetsInRange.Remove(enemy);
            }

            if (AttackTarget == enemy)
            {
                AttackTarget = null;
            }
        }

        private void OnEnemyDied(Enemy enemy)
        {
            if (_targetsInRange.Contains(enemy))
            {
                _targetsInRange.Remove(enemy);
            }

            if (AttackTarget == enemy)
            {
                AttackTarget = null;
            }
        }

        public override void LateTick()
        {
            Weapon.LateTick();

            _targetsInRange.Sort((x, y) =>
                Vector3.Distance(transform.position, x.View.transform.position)
                    .CompareTo(Vector3.Distance(transform.position, y.View.transform.position)));

            if (_targetsInRange.Count > 0)
            {
                AttackTarget = _targetsInRange[0];
            }

            if (AttackTarget != null)
            {
                AttackTargetInRange =
                    Vector3.Distance(transform.position, AttackTarget.View.transform.position) <= AttackRange;
            }
        }
    }
}
