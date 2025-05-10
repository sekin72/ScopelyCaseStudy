using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Asset.Helpers;

namespace CerberusFramework.Core.UI.Popups.RemoteAssetDownload
{
    public class RemoteAssetDownloadPopup : Popup<RemoteAssetDownloadPopupData, RemoteAssetDownloadPopupView>
    {
        public event Action ClosedItself;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.ClosedItself += OnClosedItself;
        }

        protected override void Dispose()
        {
            View.ClosedItself -= OnClosedItself;
            base.Dispose();
        }

        private void OnClosedItself()
        {
            ClosedItself?.Invoke();
        }

        public void SetDownloadProgress(AsyncOperationLoadProgress progress)
        {
            View.SetDownloadProgress(progress);
        }
    }
}