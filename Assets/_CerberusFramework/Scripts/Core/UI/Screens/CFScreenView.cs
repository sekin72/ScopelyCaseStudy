using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Components;

namespace CerberusFramework.Core.UI.Screens
{
    public abstract class CFScreenView : View, IGradualLifecycle
    {
        public Darkinator Darkinator;
        public SafeArea SafeArea;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Activate()
        {
        }

        public UniTask ActivateGradual(CancellationToken cancellationToken)
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