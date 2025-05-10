using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace CerberusFramework.Core.UI.Screens
{
    public abstract class CFScreen<TD, TV> : Controller<TD, TV>, ICFScreen
        where TD : CFScreenData
        where TV : CFScreenView
    {
        public Darkinator Darkinator => View.Darkinator;
        public SafeArea SafeArea => View.SafeArea;

        CFScreenData ICFScreen.Data => Data;
        CFScreenView ICFScreen.View => View;

        protected override UniTask Initialize(CancellationToken cancellationToken)
        {
            SafeArea.Initialize();
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Darkinator.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)SafeArea.transform);

            return UniTask.CompletedTask;
        }

        protected override void Activate()
        {
        }

        public UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override void Deactivate()
        {
        }

        public UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override void Dispose()
        {
        }
    }
}