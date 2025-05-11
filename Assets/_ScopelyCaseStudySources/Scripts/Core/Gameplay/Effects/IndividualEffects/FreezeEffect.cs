using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Effects
{
    [CreateAssetMenu(fileName = "FreezeEffect", menuName = "ScopelyCaseStudy/Effect/FreezeEffect", order = 7)]
    public class FreezeEffect : Effect
    {
        public override EffectType EffectType => EffectType.Freeze;

        public override AffectedCharacterType AffectedCharacter => _affectedCharacter;
        [SerializeField] private AffectedCharacterType _affectedCharacter = AffectedCharacterType.Enemy;

        public override AffectedComponentType AffectedComponent => AffectedComponentType.MovementComponent;

        public override string GetTitle()
        {
            return $"FREEZE";
        }

        public override string GetDescription()
        {
            return $"Movement speed decreased to -{Value}%";
        }
    }
}
