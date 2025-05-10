using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.FlyTweens
{
    public abstract class FlyTween<TD, TV> : Controller<TD, TV>, IFlyTween
        where TD : FlyTweenData
        where TV : FlyTweenView
    {
        FlyTweenData IFlyTween.Data => Data;
        FlyTweenView IFlyTween.View => View;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await View.Initialize(cancellationToken);
        }

        protected override void Activate()
        {
        }

        public async UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            await View.ActivateGradual(cancellationToken);
        }

        protected override void Deactivate()
        {
        }

        public async UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            await View.DeactivateGradual(cancellationToken);
        }

        protected override void Dispose()
        {
        }
    }
}