using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;

namespace ScopelyCaseStudy
{
    public abstract class CharacterView : View
    {
        public MovementComponent MovementComponent;
        public LifeComponent LifeComponent;
        public AttackComponent AttackComponent;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
        }
    }
}
