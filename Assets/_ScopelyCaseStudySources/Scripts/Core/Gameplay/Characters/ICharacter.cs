using CerberusFramework.Core.MVC;
using ScopelyCaseStudy.Core.Gameplay.Effects;

namespace ScopelyCaseStudy
{
    public interface ICharacter : IController
    {
        public bool IsAlive();
        public void TakeDamage(float damage);
        public void GetModified(Effect effect);
    }
}
