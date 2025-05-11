using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public interface IWeapon : ILateTickable
    {
        public AttackComponent AttackComponent { get; }
        public abstract void Initialize(AttackComponent attackComponent, WeaponConfig weaponConfig);
        public abstract void GetModified(Effect effect);
        public abstract void Dispose();
        public abstract UniTask AttackTarget(ICharacter target, CancellationToken cancellationToken);
    }
}
