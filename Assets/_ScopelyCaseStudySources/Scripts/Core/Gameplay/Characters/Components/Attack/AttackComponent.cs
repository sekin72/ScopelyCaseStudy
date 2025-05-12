using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Turrets;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using ScopelyCaseStudy.Core.Gameplay.Weapons;
using UnityEngine;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public abstract class AttackComponent : Component, ILateTickable
    {
        public event Action Attacked;
        protected ICharacter AttackTarget;
        protected float AttackRange;
        protected bool AttackTargetInRange;

        protected IWeapon Weapon;

        private float _attackCooldown;
        private bool _initialized;
        private bool _isAttacking;

        protected CancellationTokenSource _cancellationTokenSource;

        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);
            _cancellationTokenSource = new CancellationTokenSource();

            switch (CharacterConfig.WeaponConfig.GetAttackComponentType())
            {
                case AttackComponentTypes.RangedAttack:
                    Weapon = AttachedCharacter.View.gameObject.AddComponent<RangedWeapon>();
                    break;
                case AttackComponentTypes.MeleeAttack:
                    Weapon = AttachedCharacter.View.gameObject.AddComponent<MeleeWeapon>();
                    break;
            }

            Weapon.Initialize(this, characterConfig.WeaponConfig);

            _attackCooldown = 1f / characterConfig.WeaponConfig.Cooldown;
            AttackRange = characterConfig.WeaponConfig.Range;

            AttackTarget = character is Turret ? null : GameSession.Base;

            _initialized = true;
        }

        public override void Dispose()
        {
            _initialized = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            Weapon?.Dispose();

            base.Dispose();
        }

        public virtual void LateTick()
        {
            if (!_initialized)
            {
                return;
            }

            Weapon.LateTick();

            if (AttackTarget == null || !AttackTarget.IsAlive())
            {
                return;
            }

            AttackTargetInRange =
                Vector3.Distance(transform.position, AttackTarget.View.transform.position) <= AttackRange;
        }

        private async UniTask WaitUntilAttackConditionsMet()
        {
            if (!_initialized)
            {
                return;
            }

            await UniTask.WaitUntil(() =>
                AttackTarget != null &&
                AttackTarget.IsAlive() &&
                AttackTargetInRange,
            cancellationToken: _cancellationTokenSource.Token);
        }

        protected async UniTask Attack()
        {
            if (!_initialized)
            {
                return;
            }

            await WaitUntilAttackConditionsMet();

            _isAttacking = true;

            await Weapon.AttackTarget(AttackTarget, _cancellationTokenSource.Token);

            Attacked?.Invoke();

            _cancellationTokenSource?.Token.ThrowIfCancellationRequested();
            await UniTask.Delay(TimeSpan.FromSeconds(_attackCooldown), cancellationToken: _cancellationTokenSource.Token);

            _isAttacking = false;

            Attack().Forget();
        }

        public override void GetModified(Effect effect)
        {
        }

        public override void ReverseEffect(Effect effect)
        {
        }
    }
}