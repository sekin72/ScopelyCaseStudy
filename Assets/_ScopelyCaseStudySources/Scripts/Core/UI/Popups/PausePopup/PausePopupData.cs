using System;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Popups;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause
{
    public class PausePopupData : PopupData
    {
        public readonly Action OnSaveButtonClicked;
        public readonly Action OnLoadButtonClicked;
        public readonly Action OnMMButtonClicked;

        public PausePopupData(Action onSaveButtonClicked, Action onLoadButtonClicked, Action mmButtonClicked, Action onAfterClosed)
            : base(PoolKeys.PausePopup, onCloseClicked: onAfterClosed)
        {
            OnSaveButtonClicked = onSaveButtonClicked;
            OnLoadButtonClicked = onLoadButtonClicked;
            OnMMButtonClicked = mmButtonClicked;
        }
    }
}