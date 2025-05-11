using CerberusFramework.Core.MVC;

namespace ScopelyCaseStudy
{
    public abstract class CharacterData : Data
    {
        public readonly float MovementSpeed;

        public CharacterData(float movementSpeed)
        {
            MovementSpeed = movementSpeed;
        }
    }
}
