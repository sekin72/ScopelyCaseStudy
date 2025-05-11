using ScopelyCaseStudy.Core.Gameplay.Effects;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public interface IComponent
    {
        void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig);
        void Dispose();
        void GetModified(Effect effect);
    }
}
