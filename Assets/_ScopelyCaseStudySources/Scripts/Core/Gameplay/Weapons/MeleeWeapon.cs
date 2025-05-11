using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public class MeleeWeapon : MonoBehaviour, IWeapon
    {
        private PoolManager _poolManager;
        public AttackComponent AttackComponent { get; private set; }
        private float _damage;

        [Inject]
        public void Inject(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public void Initialize(AttackComponent attackComponent, WeaponConfig weaponConfig)
        {
            AttackComponent = attackComponent;
            _damage = weaponConfig.Damage;
        }

        public void Dispose()
        {
        }

        public UniTask AttackTarget(ICharacter target, CancellationToken cancellationToken)
        {
            target.TakeDamage(_damage);

            return UniTask.CompletedTask;
        }

        public void LateTick()
        {
        }

        public void GetModified(Effect effect)
        {
        }
    }
}
