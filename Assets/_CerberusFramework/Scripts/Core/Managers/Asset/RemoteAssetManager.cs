using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers.Asset.Helpers;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Core.UI.Popups.CheckConnection;
using CerberusFramework.Core.UI.Popups.RemoteAssetDownload;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CerberusFramework.Core.Managers.Asset
{
    public class RemoteAssetManager : Manager
    {
        private const string NoInternetLockID = "Remote asset download failed: No internet connection";
        private const string DownloadInProgressLockID = "Download in progress";
        private bool _catalogInitialized;

        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(RemoteAssetManager));
        private readonly Queue<AssetReference> _downloadQueue = new Queue<AssetReference>();

        private List<string> _catalogsToUpdate = new List<string>();
        private CheckConnectionPopup _checkConnectionPopup;

        private readonly LockBin _downloadQueueBlockerLockBin = new LockBin();

        private bool _isConnected;

        private readonly LockBin _outsideDownloadInProgress = new LockBin();

        private IPublisher<RemoteAssetDependencyDownloadStatusChangedEvent> _remoteAssetDependencyDownloadStatusChangedEventPublisher;

        public override bool IsCore => true;
        public string CatalogHash { get; private set; } = string.Empty;

        private event Action CheckYourConnectionPopupOpened;

        private event Action OnConnectionLost;

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            using (PerformanceUtils.CFStopwatch("Initialize Addressable"))
            {
                await Addressables.InitializeAsync();
            }

            DownloadQueue.Clear();
            DependencyDictionary.Clear();

            _remoteAssetDependencyDownloadStatusChangedEventPublisher = GlobalMessagePipe.GetPublisher<RemoteAssetDependencyDownloadStatusChangedEvent>();

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Logger.Warn(NoInternetLockID);
                _downloadQueueBlockerLockBin.Increase(NoInternetLockID);
                _isConnected = false;
            }
            else
            {
                _isConnected = true;

                if (!_catalogInitialized && !await CheckAndUpdateCatalog(disposeToken))
                {
                    Logger.Warn("Check catalog failed, hit timeout. Using the old catalog");
                }
            }

            CatalogHash = Addressables.GetLocatorInfo(Addressables.ResourceLocators.ElementAt(0)).LocalHash;
            _catalogInitialized = true;

            SetReady();

            StartQueueDownload(disposeToken).Forget();
        }

        private async UniTask<bool> CheckAndUpdateCatalog(CancellationToken cancellationToken)
        {
            try
            {
                var timedOut = false;
                var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken).ContinueWith(() => timedOut = true);
                var checkTask = CheckCatalogs(cancellationToken);

                await UniTask.WhenAny(checkTask, timeoutTask);

                if (timedOut)
                {
                    throw new OperationCanceledException();
                }

                if (_catalogsToUpdate.Count > 0)
                {
                    Logger.Debug($"Updating catalogs: {string.Join(", ", _catalogsToUpdate)}");
                    using (PerformanceUtils.CFStopwatch("UpdateCatalogs"))
                    {
                        await UpdateCatalogs(cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return false;
            }

            return true;
        }

        private async UniTask CheckCatalogs(CancellationToken cancellationToken)
        {
            _catalogsToUpdate = new List<string>();
            AsyncOperationHandle<List<string>> checkForUpdateHandle = new AsyncOperationHandle<List<string>>();

            try
            {
                checkForUpdateHandle = Addressables.CheckForCatalogUpdates(false);
                await checkForUpdateHandle;
                cancellationToken.ThrowIfCancellationRequested();

                if (checkForUpdateHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    if (checkForUpdateHandle.Result != null && checkForUpdateHandle.Result.Count != 0)
                    {
                        _catalogsToUpdate.AddRange(checkForUpdateHandle.Result);
                    }
                    else
                    {
                        Logger.Debug("No Content Catalog to update. All good!");
                    }
                }
                else
                {
                    Logger.Error($"Failed to check for Content Catalog updates: Status={checkForUpdateHandle.Status}, Exception={checkForUpdateHandle.OperationException}");
                }

                Addressables.Release(checkForUpdateHandle);
            }
            catch (OperationCanceledException)
            {
                Addressables.Release(checkForUpdateHandle);
                throw;
            }
            catch (Exception e)
            {
                Addressables.Release(checkForUpdateHandle);
                Logger.Warn($"Check catalog failed {e.Message}");
            }
        }

        private async UniTask<bool> UpdateCatalogs(CancellationToken cancellationToken)
        {
            AsyncOperationHandle<List<IResourceLocator>> updateHandle = new AsyncOperationHandle<List<IResourceLocator>>();

            try
            {
                updateHandle = Addressables.UpdateCatalogs(_catalogsToUpdate, false);
                await updateHandle;
                cancellationToken.ThrowIfCancellationRequested();

                var success = updateHandle.Status == AsyncOperationStatus.Succeeded;

                if (success)
                {
                    if (updateHandle.Result?.Count > 0)
                    {
                        var locatorsIds = updateHandle.Result.Select(r => r.LocatorId);
                        var locatorsEnumerable = locatorsIds as string[] ?? locatorsIds.ToArray();
                        Logger.Debug(
                            $"Catalogs was updated successfully for {locatorsEnumerable.Length} locators: {string.Join(", ", locatorsEnumerable)}"
                        );
                    }
                    else
                    {
                        Logger.Debug("Catalogs was updated with no results.");
                    }
                }
                else
                {
                    Logger.Warn($"Unable to update Catalogs: Status={updateHandle.Status}, Exception={updateHandle.OperationException}");
                }

                Addressables.Release(updateHandle);

                return success;
            }
            catch (OperationCanceledException)
            {
                Addressables.Release(updateHandle);
                throw;
            }
            catch (Exception e)
            {
                Addressables.Release(updateHandle);
                Logger.Warn($"Update Catalogs failed {e.Message}");
            }

            return false;
        }

        private void AddRemoteAssetToDictionaryAsUnknown(AssetReference assetReference, string displayNameForDebugging)
        {
            if (DependencyDictionary.ContainsKey(assetReference))
            {
                return;
            }

            displayNameForDebugging ??= assetReference.RuntimeKey.ToString();

            var dependencyInfo = new DependencyInfo(
                assetReference,
                displayNameForDebugging,
                -1,
                -1,
                DependencyDownloadStatus.Unknown
            );

            SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Unknown);

            DependencyDictionary.Add(assetReference, dependencyInfo);
        }

        private async UniTask CheckRemoteAssetSizeForDictionary(AssetReference assetReference, CancellationToken cancellationToken)
        {
            var dependencyInfo = GetDependencyInfo(assetReference);
            if (!dependencyInfo.DependencyDownloadStatus.NeedsChecking())
            {
                return;
            }

            AsyncOperationHandle bytesToDownloadHandle = new AsyncOperationHandle();

            try
            {
                bytesToDownloadHandle = Addressables.GetDownloadSizeAsync(assetReference);
                await bytesToDownloadHandle;
                cancellationToken.ThrowIfCancellationRequested();

                var bytesToDownload = Convert.ToInt64(bytesToDownloadHandle.Result);
                dependencyInfo.SizeAtAppStart = bytesToDownload;
                dependencyInfo.SizeUpdated = bytesToDownload;

                SetDownloadStatus(dependencyInfo,
                    dependencyInfo.SizeUpdated == 0
                        ? DependencyDownloadStatus.InBuild
                        : dependencyInfo.SizeUpdated == dependencyInfo.SizeAtAppStart
                            ? DependencyDownloadStatus.Queued
                            : DependencyDownloadStatus.Downloading);

                DependencyDictionary[assetReference] = dependencyInfo;
                Addressables.Release(bytesToDownloadHandle);
            }
            catch (OperationCanceledException)
            {
                Addressables.Release(bytesToDownloadHandle);
                SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);
                throw;
            }
            catch (Exception e)
            {
                Addressables.Release(bytesToDownloadHandle);
                SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);
                Logger.Warn($"CheckBundleSizeForDictionary exception {e.Message}");
            }
        }

        private async UniTask StartQueueDownload(CancellationToken cancellationToken)
        {
            if (_downloadQueueBlockerLockBin || _downloadQueue.Count == 0)
            {
                await UniTask.WaitUntil(() => !_downloadQueueBlockerLockBin && _downloadQueue.Count > 0, cancellationToken: cancellationToken);
            }

            if (_downloadQueue.Count > 0)
            {
                var assetReference = _downloadQueue.Dequeue();
                var dependencyInfo = GetDependencyInfo(assetReference);
                dependencyInfo.SetAsyncOperationLoadProgress();
                await DownloadRemoteAsset(assetReference, false);

                if (dependencyInfo.DependencyDownloadStatus.NeedsDownload())
                {
                    _downloadQueue.Enqueue(assetReference);
                }
            }

            StartQueueDownload(cancellationToken).Forget();
        }

        public async UniTask EnqueueRemoteAsset(AssetReference assetReference, CancellationToken cancellationToken, string displayNameForDebugging = null)
        {
            if (!DependencyDictionary.ContainsKey(assetReference))
            {
                AddRemoteAssetToDictionaryAsUnknown(assetReference, displayNameForDebugging);
            }

            if (_downloadQueue.Contains(assetReference))
            {
                return;
            }

            var dependencyInfo = GetDependencyInfo(assetReference);

            if (dependencyInfo.DependencyDownloadStatus.NeedsChecking())
            {
                await CheckRemoteAssetSizeForDictionary(assetReference, cancellationToken);
            }

            if (dependencyInfo.DependencyDownloadStatus.NeedsDownload())
            {
                SetDownloadStatus(assetReference, DependencyDownloadStatus.Queued);
                _downloadQueue.Enqueue(assetReference);

                if (!DownloadQueue.Contains(assetReference))
                {
                    DownloadQueue.Add(assetReference);
                }
            }
        }

        public async UniTask<bool> RequestRemoteAsset(
            AssetReference assetReference,
            bool tryWaitUntilDownloaded,
            CancellationToken cancellationToken,
            Action onNotReachable = null,
            PopupManager popupManager = null, //showDownloadPopup
            string displayNameForDebugging = null,
            Action onConnectionLost = null)
        {
            if (!DependencyDictionary.ContainsKey(assetReference))
            {
                AddRemoteAssetToDictionaryAsUnknown(assetReference, displayNameForDebugging);
            }

            var lockKey = assetReference.RuntimeKey.ToString();

            CFButton.DisableInput($"Preparing Asset {assetReference.RuntimeKey}");
            _outsideDownloadInProgress.Increase(lockKey);

            var dependencyInfo = GetDependencyInfo(assetReference);
            try
            {
                if (dependencyInfo.DependencyDownloadStatus.NeedsChecking())
                {
                    await CheckRemoteAssetSizeForDictionary(assetReference, cancellationToken);
                }
            }
            catch (Exception)
            {
                _outsideDownloadInProgress.Decrease(lockKey);
                throw;
            }
            finally
            {
                CFButton.EnableInput($"Preparing Asset {assetReference.RuntimeKey}");
            }

            if (!IsAssetReachable(assetReference))
            {
                onNotReachable?.Invoke();

                if (tryWaitUntilDownloaded)
                {
                    OpenCloseCheckYourConnectionPopup(_isConnected, popupManager, cancellationToken).Forget();

                    try
                    {
                        await UniTask.WaitUntil(() => _checkConnectionPopup == null, cancellationToken: cancellationToken);
                    }
                    catch (Exception)
                    {
                        _outsideDownloadInProgress.Decrease(lockKey);
                        throw;
                    }
                }
                else
                {
                    _outsideDownloadInProgress.Decrease(lockKey);
                    return false;
                }
            }

            if (!dependencyInfo.DependencyDownloadStatus.NeedsDownload())
            {
                _outsideDownloadInProgress.Decrease(lockKey);
                return true;
            }

            OnConnectionLost += onConnectionLost;
            SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Queued);
            try
            {
                do
                {
                    await DownloadRemoteAsset(assetReference, tryWaitUntilDownloaded, popupManager);
                    cancellationToken.ThrowIfCancellationRequested();
                } while (tryWaitUntilDownloaded && dependencyInfo.DependencyDownloadStatus.NeedsDownload());
            }
            finally
            {
                _outsideDownloadInProgress.Decrease(lockKey);
            }

            OnConnectionLost -= onConnectionLost;
            return !dependencyInfo.DependencyDownloadStatus.NeedsDownload();
        }

        private async UniTask OpenCloseCheckYourConnectionPopup(bool isConnected, PopupManager popupManager, CancellationToken cancellationToken)
        {
            switch (isConnected)
            {
                case true when _checkConnectionPopup != null:
                    popupManager.Close(_checkConnectionPopup);
                    _checkConnectionPopup = null;
                    break;
                case false when _checkConnectionPopup != null:
                    {
                        if (!_outsideDownloadInProgress)
                        {
                            return;
                        }

                        OnConnectionLost?.Invoke();

                        await popupManager.Open<CheckConnectionPopup, CheckConnectionPopupData, CheckConnectionPopupView>(
                            new CheckConnectionPopupData(false, null, () => _checkConnectionPopup = null)
                            , PopupShowActions.HideAll, cancellationToken);

                        _checkConnectionPopup = popupManager.GetPopup<CheckConnectionPopup>() as CheckConnectionPopup;

                        CheckYourConnectionPopupOpened?.Invoke();
                        break;
                    }
            }
        }

        private async UniTask<bool> DownloadRemoteAsset(AssetReference assetReference, bool tryWaitUntilDownloaded, PopupManager popupManager = null)
        {
            var closedItself = false;
            var timedOut = false;

            Logger.Warn($"Asset {assetReference.RuntimeKey} is not downloaded yet but tried to load");

            var dependencyInfo = GetDependencyInfo(assetReference);
            dependencyInfo.SetAsyncOperationLoadProgress();
            RemoteAssetDownloadPopup popup = null;
            var popupClosedByButton = false;

            if (popupManager != null)
            {
                CFButton.DisableInput($"Downloading Asset {assetReference.RuntimeKey}");

                if (!tryWaitUntilDownloaded)
                {
                    CheckYourConnectionPopupOpened += () =>
                    {
                        popup = (RemoteAssetDownloadPopup)popupManager.GetPopup<RemoteAssetDownloadPopup>();
                        if (popup == null)
                        {
                            return;
                        }

                        popupManager.Close(popup);
                        closedItself = true;
                    };
                }

                await popupManager.Open<RemoteAssetDownloadPopup, RemoteAssetDownloadPopupData, RemoteAssetDownloadPopupView>(
                    new RemoteAssetDownloadPopupData(() => popupClosedByButton = true), PopupShowActions.CloseAll, DisposeToken);

                popup = (RemoteAssetDownloadPopup)popupManager.GetPopup<RemoteAssetDownloadPopup>();
                popup.SetCloseButtonVisible(false);
                popup.ClosedItself += () => closedItself = true;
                popup.SetDownloadProgress(dependencyInfo.AsyncOperationLoadProgress);
            }

            var timeoutTask = UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: DisposeToken).ContinueWith(() => timedOut = true);
            var downloadTask = DownloadRemoteAssetInternal(assetReference, DisposeToken);

            await UniTask.WhenAny(downloadTask, timeoutTask);

            if (timedOut)
            {
                SetDownloadStatus(assetReference, DependencyDownloadStatus.Timeout);
            }

            if (popupManager != null)
            {
                CFButton.EnableInput($"Downloading Asset {assetReference.RuntimeKey}");

                if (!closedItself && (timedOut || dependencyInfo.DependencyDownloadStatus.NeedsDownload()))
                {
                    if (!tryWaitUntilDownloaded)
                    {
                        popup.SetCloseButtonVisible(true);
                    }

                    await UniTask.WaitUntil(() => closedItself || popupClosedByButton, cancellationToken: DisposeToken);
                    await UniTask.DelayFrame(1, cancellationToken: DisposeToken);
                }
            }

            CheckYourConnectionPopupOpened = null;

            return !dependencyInfo.DependencyDownloadStatus.NeedsDownload();
        }

        private async UniTask DownloadRemoteAssetInternal(AssetReference assetReference, CancellationToken cancellationToken)
        {
            var dependencyInfo = GetDependencyInfo(assetReference);

            _downloadQueueBlockerLockBin.Increase(DownloadInProgressLockID);

            SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Downloading);

            AsyncOperationHandle handle = default;
            try
            {
                if (!dependencyInfo.AsyncOperationLoadProgress.HandleSet)
                {
                    handle = Addressables.DownloadDependenciesAsync(assetReference);
                    dependencyInfo.AsyncOperationLoadProgress.SetHandle(handle);
                }

                await UniTask.WaitUntil(
                    () => dependencyInfo.AsyncOperationLoadProgress.IsDone, cancellationToken: cancellationToken
                );

                if (dependencyInfo.AsyncOperationLoadProgress.Succeeded)
                {
                    dependencyInfo.SizeUpdated = 0;
                    SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Downloaded);
                }
                else if (dependencyInfo.AsyncOperationLoadProgress?.Handle.OperationException != null)
                {
                    throw dependencyInfo.AsyncOperationLoadProgress.Handle.OperationException;
                }
                else
                {
                    SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);
                    Logger.Warn($"Error while downloading {dependencyInfo.DisplayNameForDebugging}");
                }
            }
            catch (OperationCanceledException)
            {
                SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);
                throw;
            }
            catch (InvalidKeyException)
            {
                SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);
                Logger.Error($"Asset is not marked as addressable {dependencyInfo.DisplayNameForDebugging}");
                throw;
            }
            catch (Exception e)
            {
                SetDownloadStatus(dependencyInfo, DependencyDownloadStatus.Exception);

                if (!AddressableManager.IsRemoteException(e))
                {
                    throw;
                }
            }
            finally
            {
                Addressables.Release(handle);
            }

            _downloadQueueBlockerLockBin.Decrease(DownloadInProgressLockID);
        }

        public bool NeedsDownload(AssetReference assetReference)
        {
            return GetDependencyInfo(assetReference).DependencyDownloadStatus.NeedsDownload();
        }

        public DependencyInfo GetDependencyInfo(AssetReference assetReference, bool @throw = true)
        {
            if (DependencyDictionary.TryGetValue(assetReference, out var dependencyInfo) || !@throw)
            {
                return dependencyInfo;
            }

            Logger.Error($"DependencyDictionary doesn't contain {assetReference.RuntimeKey}");
            throw new Exception($"DependencyDictionary doesn't contain {assetReference.RuntimeKey}");
        }

        private void SetDownloadStatus(AssetReference assetReference, DependencyDownloadStatus downloadStatus)
        {
            var dependencyInfo = GetDependencyInfo(assetReference);
            SetDownloadStatus(dependencyInfo, downloadStatus);
        }

        private void SetDownloadStatus(DependencyInfo dependencyInfo, DependencyDownloadStatus downloadStatus)
        {
            dependencyInfo.DependencyDownloadStatus = downloadStatus;
            _remoteAssetDependencyDownloadStatusChangedEventPublisher.Publish(new RemoteAssetDependencyDownloadStatusChangedEvent(dependencyInfo));
        }

        private bool IsAssetReachable(AssetReference assetReference)
        {
            var status = GetDependencyInfo(assetReference).DependencyDownloadStatus;

            return status.IsPresent() || _isConnected;
        }

        public Dictionary<AssetReference, DependencyInfo> DependencyDictionary { get; } = new Dictionary<AssetReference, DependencyInfo>();

        public List<AssetReference> DownloadQueue = new List<AssetReference>();

        public async UniTask ClearAllDownloads(CancellationToken cancellationToken)
        {
            for (var i = 0; i < DependencyDictionary.Count; i++)
            {
                var key = DependencyDictionary.Keys.ElementAt(i);
                try
                {
                    Addressables.Release(key);
                }
                catch (Exception)
                {
                    //Ignore
                }

                await Addressables.ClearDependencyCacheAsync(key, true);
                cancellationToken.ThrowIfCancellationRequested();
            }

            DownloadQueue.Clear();
            DependencyDictionary.Clear();
        }

        public async UniTask CheckRemoteAssetDebug(AssetReference assetReference, string displayNameForDebugging, CancellationToken cancellationToken)
        {
            var dependencyInfo = GetDependencyInfo(assetReference, false);

            if (dependencyInfo == null)
            {
                AddRemoteAssetToDictionaryAsUnknown(assetReference, displayNameForDebugging);
                dependencyInfo = GetDependencyInfo(assetReference);
            }

            if (dependencyInfo.DependencyDownloadStatus.NeedsChecking())
            {
                await CheckRemoteAssetSizeForDictionary(assetReference, cancellationToken);
            }
        }
    }
}