using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public class CreepAttackComponent : AttackComponent
    {
        public override void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            base.Initialize(gameSession, character, characterConfig);

            Attack().Forget();
        }
    }
}
