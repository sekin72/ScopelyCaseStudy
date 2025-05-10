using System;
using JetBrains.Annotations;
using CerberusFramework.Core.Managers.Asset.Helpers;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace CerberusFramework.Core.UI.Popups.RemoteAssetDownload
{
    public class RemoteAssetDownloadPopupView : PopupView
    {
        [SerializeField] protected CFText ProgressText;
        [SerializeField] protected Slider ProgressSlider;

        public event Action ClosedItself;

        private AsyncOperationLoadProgress _downloadProgress;

        public void SetDownloadProgress(AsyncOperationLoadProgress asyncOperationLoadProgress)
        {
            _downloadProgress = asyncOperationLoadProgress;
        }

        [UsedImplicitly]
        private void Update()
        {
            if (_downloadProgress == null)
            {
                return;
            }

            if (_downloadProgress.Succeeded)
            {
                ClosedItself?.Invoke();
                return;
            }

            if (_downloadProgress.IsDone)
            {
                return;
            }

            ProgressSlider.value = _downloadProgress.LoadProgress();
            ProgressText.Text = _downloadProgress.GetDescription();
        }
    }
}