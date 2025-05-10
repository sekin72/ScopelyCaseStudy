using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.Popups
{
    public abstract class Popup<TD, TV> : Controller<TD, TV>, IPopup
        where TD : PopupData
        where TV : PopupView
    {
        public string UniqueName { get; private set; }

        #region Lifecycle

        PopupData IPopup.Data => Data;
        PopupView IPopup.View => View;

        protected override UniTask Initialize(CancellationToken cancellationToken)
        {
            UniqueName = $"{GetType().Name}";
            View.CloseButtonClicked += ClosePopup;

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// This method is for FORCE ACTIVATION. Use carefully.
        /// </summary>
        protected override void Activate()
        {
            View.SetVisible(true);
        }

        /// <summary>
        /// This method is for FORCE DEACTIVATION. Use carefully.
        /// </summary>
        protected override void Deactivate()
        {
            View.SetVisible(false);
            View.CloseButtonClicked -= ClosePopup;
        }

        public UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            Deactivate();
            return UniTask.CompletedTask;
        }

        protected override void Dispose()
        {
        }

        #endregion Lifecycle

        public void SetCloseButtonVisible(bool isVisible)
        {
            View.SetCloseButtonVisible(isVisible);
        }

        public virtual void TapOutside()
        {
            ClosePopup();
        }

        public virtual void GoBack()
        {
            ClosePopup();
        }

        protected void ClosePopup()
        {
            Data.OnCloseClicked?.Invoke();
            Data.CloseCall?.Invoke(this);
        }
    }
}