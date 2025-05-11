using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public class TurretAttackComponent : AttackComponent
    {
        [SerializeField] private TurretColliderProxy _colliderProxy;
        private IEnemyControllerSystem _enemyControllerSystem;
        private List<ICharacter> _targetsInRange;

        private IDisposable _messageSubscription;

        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);

            _enemyControllerSystem = GameSession.GetSystem<IEnemyControllerSystem>();

            _targetsInRange = new List<ICharacter>();

            _colliderProxy.OnEnemyCollided += OnEnemyCollided;
            _colliderProxy.OnEnemyExited += OnEnemyExited;
            _colliderProxy.SetColliderRadius(AttackRange);

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<EnemyKilledEvent>().Subscribe(OnEnemyDied).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            Attack().Forget();
        }

        public override void Dispose()
        {
            _messageSubscription?.Dispose();

            _colliderProxy.OnEnemyCollided -= OnEnemyCollided;
            _colliderProxy.OnEnemyExited -= OnEnemyExited;

            base.Dispose();
        }

        private void OnEnemyCollided(EnemyView enemyView)
        {
            var enemy = _enemyControllerSystem.GetEnemyFromView(enemyView);
            _targetsInRange.Add(enemy);
        }

        private void OnEnemyExited(EnemyView enemyView)
        {
            var enemy = _enemyControllerSystem.GetEnemyFromView(enemyView);

            if (_targetsInRange.Contains(enemy))
            {
                _targetsInRange.Remove(enemy);
            }

            if (AttackTarget == enemy)
            {
                AttackTarget = null;
            }
        }

        private void OnEnemyDied(EnemyKilledEvent evt)
        {
            var enemy = evt.Enemy;
            if (!_targetsInRange.Contains(enemy))
            {
                return;
            }

            _targetsInRange.Remove(enemy);

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
