using System;
using CerberusFramework.Core.Managers.Pool;

namespace CerberusFramework.Core.UI.Popups.CheckConnection
{
    public class CheckConnectionPopupData : PopupData
    {
        public readonly bool ShowConfirmButton;
        public readonly Action OnConfirmed;
        public readonly Action OnClosed;

        public CheckConnectionPopupData(bool showConfirmButton, Action onConfirmed, Action onClosed)
            : base(PoolKeys.CheckYourConnectionPopup)
        {
            ShowConfirmButton = showConfirmButton;
            OnConfirmed = onConfirmed;
            OnClosed = onClosed;
        }
    }
}