using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Components
{
    public abstract class Component : MonoBehaviour, IComponent
    {
        public ICharacter AttachedCharacter;
        protected CharacterConfig CharacterConfig;

        protected GameSession GameSession;

        public virtual void Initialize(GameSession gameSession, ICharacter character, CharacterConfig characterConfig)
        {
            GameSession = gameSession;
            AttachedCharacter = character;
            CharacterConfig = characterConfig;
        }

        public virtual void Dispose()
        {
        }

        public abstract void GetModified(Effect effect);
    }
}
