using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public abstract class MovementComponent : Component
    {
        [SerializeField] protected Transform _modelTransform;
        protected float MoveSpeed;

        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);
            MoveSpeed = CharacterConfig.MoveSpeed;
        }
        public abstract void LateTick();

        public override void GetModified(Effect effect)
        {
            switch (effect.EffectType)
            {
                case EffectType.Freeze:
                    MoveSpeed *= 1 - (effect.Value / 100f);
                    break;
            }
        }
    }
}
