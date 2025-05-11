using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Effects
{
    public abstract class Effect : ScriptableObject
    {
        public abstract EffectType EffectType { get; }

        public abstract AffectedCharacterType AffectedCharacter { get; }
        public abstract AffectedComponentType AffectedComponent { get; }

        public float Value;

        public abstract string GetTitle();
        public abstract string GetDescription();
    }

    public enum AffectedCharacterType
    {
        Player,
        Enemy
    }

    public enum AffectedComponentType
    {
        MovementComponent,
        LifeComponent,
        AttackComponent,
    }
}
