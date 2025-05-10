using System;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.Popups
{
    public abstract class PopupData : Data
    {
        public readonly PoolKeys PoolKey;
        public readonly bool ShowDarkinator;
        public Action OnCloseClicked { get; private set; }
        public Action<IPopup> CloseCall { get; private set; }

        protected PopupData(PoolKeys poolKey,
            bool showDarkinator = true,
            Action onCloseClicked = null)
        {
            PoolKey = poolKey;
            ShowDarkinator = showDarkinator;
            OnCloseClicked = onCloseClicked;
        }

        public void AttachCloseCall(Action<IPopup> closeCall)
        {
            CloseCall = closeCall;
        }
    }
}