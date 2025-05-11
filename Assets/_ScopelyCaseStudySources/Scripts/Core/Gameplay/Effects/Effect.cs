using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Effects
{
    public abstract class Effect : ScriptableObject
    {
        public abstract EffectType EffectType { get; }
        public abstract AffectedCharacterType AffectedCharacter { get; }
        public abstract AffectedComponentType AffectedComponent { get; }

        public float Value;
        public float Time;

        public abstract UniTask ApplyEffect(CancellationToken cancellationToken);
    }

    public enum AffectedCharacterType
    {
        Base,
        Enemy,
        Turret,
    }

    public enum AffectedComponentType
    {
        MovementComponent,
        LifeComponent,
        AttackComponent,
    }
}
