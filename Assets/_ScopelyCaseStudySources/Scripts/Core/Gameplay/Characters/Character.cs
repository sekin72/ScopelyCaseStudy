using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy
{
    public abstract class Character<TD, TV> : Controller<TD, TV>, ICharacter, ILateTickable 
        where TV : CharacterView 
        where TD : CharacterData
    {
        protected MovementComponent MovementComponent { get; set; }
        protected LifeComponent LifeComponent { get; set; }
        protected AttackComponent AttackComponent { get; set; }

        protected GameSession GameSession;

        public void SetSession(GameSession gameSession)
        {
            GameSession = gameSession;
        }

        protected override UniTask Initialize(CancellationToken cancellationToken)
        {
            MovementComponent = View.MovementComponent;
            LifeComponent = View.LifeComponent;
            AttackComponent = View.AttackComponent;
            LifeComponent.Died += OnDeath;

            return UniTask.CompletedTask;
        }

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
        }

        protected override void Dispose()
        {
            LifeComponent.Died -= OnDeath;
            MovementComponent.Dispose();
            LifeComponent.Dispose();
            AttackComponent.Dispose();
        }
        public abstract void OnDeath();

        public bool IsAlive()
        {
            return LifeComponent.IsAlive();
        }

        public void TakeDamage(float damage)
        {
            LifeComponent.TakeDamage(damage);
        }

        public void LateTick()
        {
            MovementComponent.LateTick();
            LifeComponent.LateTick();
            AttackComponent.LateTick();
        }

        public void GetModified(Effect effect)
        {
            switch (effect.AffectedComponent)
            {
                case AffectedComponentType.MovementComponent:
                    MovementComponent.GetModified(effect);
                    break;
                case AffectedComponentType.LifeComponent:
                    LifeComponent.GetModified(effect);
                    break;
                case AffectedComponentType.AttackComponent:
                    AttackComponent.GetModified(effect);
                    break;
            }
        }
    }
}
