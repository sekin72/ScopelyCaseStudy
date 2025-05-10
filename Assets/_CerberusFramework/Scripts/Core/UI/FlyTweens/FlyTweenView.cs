using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.FlyTweens
{
    public abstract class FlyTweenView : View, IGradualLifecycle
    {
        private static ulong _nameIdCounter;
        public Ease AnimationEase = Ease.InCirc;
        public float DurationInSeconds = 0.5f;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            gameObject.name = GetType().Name + "-" + ++_nameIdCounter;

            return UniTask.CompletedTask;
        }

        public override void Activate()
        {
        }

        public virtual UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Deactivate()
        {
        }

        public UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
        }
    }
}