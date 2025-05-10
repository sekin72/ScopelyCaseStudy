using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Utilities.Logging;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace CerberusFramework.Core.Managers.Asset
{
    public class AddressableManager : Manager
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(AddressableManager));

        public override bool IsCore => true;
        public PoolDataHolder PoolDataHolder { get; private set; }
        private AsyncOperationHandle _poolDataHolderHandle;

        private bool _isInitialized;

        private RemoteAssetManager _remoteAssetManager;

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>
            {
                _remoteAssetManager
            };
        }

        [Inject]
        public void Inject(RemoteAssetManager remoteAssetManager)
        {
            _remoteAssetManager = remoteAssetManager;
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            if (PoolDataHolder == null)
            {
                (PoolDataHolder, _poolDataHolderHandle) = await LoadAssetAsync<PoolDataHolder>("PoolDataHolder", disposeToken, false);
            }

            _isInitialized = true;

            SetReady();
        }

        public override void Dispose()
        {
            if (PoolDataHolder != null)
            {
                ReleaseInstance(_poolDataHolderHandle);
            }

            PoolDataHolder = null;

            base.Dispose();
        }

        public async UniTask<SceneInstance> LoadSceneAdditive(object key, CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            SceneInstance sceneInstance = default;
            var isLoaded = false;
            try
            {
                sceneInstance = await Addressables.LoadSceneAsync(key, LoadSceneMode.Additive, false);
                isLoaded = true;
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                if (isLoaded)
                {
                    await Addressables.UnloadSceneAsync(sceneInstance);
                }

                throw;
            }

            return sceneInstance;
        }

        public async UniTask UnloadScene(SceneInstance sceneInstance, CancellationToken cancellationToken)
        {
            if (!_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            await Addressables.UnloadSceneAsync(sceneInstance);
            cancellationToken.ThrowIfCancellationRequested();
        }

        public async UniTask<(T, AsyncOperationHandle)> LoadAssetAsync<T>(AssetReferenceT<T> assetReference,
            CancellationToken cancellationToken = default, bool waitInitialize = true)
            where T : Object
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            return await LoadAssetAsyncInternal<T>(assetReference, cancellationToken);
        }

        public async UniTask<(T, AsyncOperationHandle)> LoadAssetAsync<T>(string assetPath,
            CancellationToken cancellationToken = default, bool waitInitialize = true)
            where T : Object
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            return await LoadAssetAsyncInternal<T>(assetPath, cancellationToken);
        }

        public async UniTask<GameObject> InstantiateAssetAsync(string name, CancellationToken cancellationToken,
            Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null, IObjectResolver resolver = null, bool waitInitialize = true)
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            return await InstantiateAssetAsyncInternal(
                name, cancellationToken, localPosition, localQuaternion, localScale, parent, resolver
            );
        }

        public async UniTask<GameObject> InstantiateAssetAsync(AssetReferenceT<GameObject> assetReference,
            CancellationToken cancellationToken, Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null, IObjectResolver resolver = null, bool waitInitialize = true)
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            return await InstantiateAssetAsyncInternal(
                assetReference, cancellationToken, localPosition, localQuaternion, localScale, parent, resolver
            );
        }

        public bool ReleaseInstance(GameObject gameObject)
        {
            if (_isInitialized)
            {
                return Addressables.ReleaseInstance(gameObject);
            }

            Logger.Error("Addressables is not initialized");
            return false;

        }

        public bool ReleaseInstance(AsyncOperationHandle handle)
        {
            if (_isInitialized)
            {
                return Addressables.ReleaseInstance(handle);
            }

            Logger.Error("Addressables is not initialized");
            return false;

        }

        public void ReleaseInstance<T>(T t) where T : Object
        {
            if (!_isInitialized)
            {
                Logger.Error("Addressables is not initialized");
                return;
            }

            Addressables.Release(t);
        }

        public async UniTask<T> LoadRemoteAssetAsync<T>(AssetReferenceT<T> assetReference,
            CancellationToken cancellationToken = default, bool waitInitialize = true)
            where T : Object
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            if (!await _remoteAssetManager.RequestRemoteAsset(
                assetReference,
                true,
                cancellationToken))
            {
                return null;
            }

            var (asset, _) = await LoadAssetAsyncInternal<T>(assetReference, cancellationToken);

            if (asset != null)
            {
                _remoteAssetManager.GetDependencyInfo(assetReference).SetDisplayName(asset.name);
            }

            return asset;
        }

        public async UniTask<GameObject> InstantiateRemoteAssetAsync(AssetReferenceT<GameObject> assetReference, bool tryWaitUntilDownloaded,
            CancellationToken cancellationToken, Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null, IObjectResolver resolver = null, bool waitInitialize = true)
        {
            if (waitInitialize && !_isInitialized)
            {
                await UniTask.WaitUntil(() => _isInitialized, cancellationToken: cancellationToken);
            }

            if (!await _remoteAssetManager.RequestRemoteAsset(
                assetReference,
                tryWaitUntilDownloaded,
                cancellationToken))
            {
                return null;
            }

            var created = await InstantiateAssetAsync(
                assetReference, cancellationToken, localPosition, localQuaternion, localScale, parent, resolver,
                waitInitialize
            );

            if (created != null)
            {
                _remoteAssetManager.GetDependencyInfo(assetReference).SetDisplayName(created.name);
            }

            return created;
        }

        #region Internal

        private static async UniTask<(T, AsyncOperationHandle)> LoadAssetAsyncInternal<T>(object assetLocation,
            CancellationToken cancellationToken) where T : Object
        {
            T asset = null;
            AsyncOperationHandle handle = default;
            try
            {
                handle = Addressables.LoadAssetAsync<T>(assetLocation);
                await handle.ToUniTask(cancellationToken: cancellationToken)
                    .ContinueWith(() => asset = handle.Result as T);

            }
            catch (Exception e)
            {
                if (asset != null)
                {
                    Addressables.Release(asset);
                    asset = null;
                }

                if (e is OperationCanceledException)
                {
                    throw;
                }

                if (!IsRemoteException(e))
                {
                    throw;
                }
            }

            return (asset, handle);
        }

        private static async UniTask<GameObject> InstantiateAsyncInternal(object assetLocation,
            InstantiationParameters parameters, CancellationToken cancellationToken)
        {
            GameObject created = null;
            try
            {
                created = await Addressables.InstantiateAsync(assetLocation, parameters);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                if (created != null)
                {
                    Addressables.ReleaseInstance(created);
                    created = null;
                }

                if (e is OperationCanceledException)
                {
                    throw;
                }

                if (!IsRemoteException(e))
                {
                    throw;
                }
            }

            return created;
        }

        private static async UniTask<GameObject> InstantiateAssetAsyncInternal(object assetReference,
            CancellationToken cancellationToken, Vector3 localPosition = default, Quaternion localQuaternion = default,
            Vector3 localScale = default, Transform parent = null, IObjectResolver resolver = null)
        {
            var instantiateParameters = new InstantiationParameters(parent, false);
            var created = await InstantiateAsyncInternal(assetReference, instantiateParameters, cancellationToken);
            if (created == null)
            {
                return null;
            }

            created.transform.localPosition = localPosition;
            created.transform.localRotation = localQuaternion;
            created.transform.localScale = localScale;

            resolver?.InjectGameObject(created);

            return created;
        }

        public static bool IsRemoteException(Exception e)
        {
            var message = e;
            var logged = false;
            do
            {
                if (message.Message.Contains("RemoteProviderException"))
                {
                    logged = true;
                    Logger.Error(message.Message);
                    break;
                }

                message = message.InnerException;
            } while (message != null);

            return logged;
        }

        #endregion Internal
    }
}