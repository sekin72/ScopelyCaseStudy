using System;
using CerberusFramework.Core.Managers.Pool;

namespace CerberusFramework.Core.UI.Popups.RemoteAssetDownload
{
    public class RemoteAssetDownloadPopupData : PopupData
    {
        public RemoteAssetDownloadPopupData(Action onCloseClicked) : base(PoolKeys.RemoteAssetDownloadPopup, onCloseClicked: onCloseClicked)
        {
        }
    }
}