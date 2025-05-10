using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Config;
using CerberusFramework.Core.Managers.Asset;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace CerberusFramework.Core.Managers.Data
{
    public sealed class AssetManager : Manager
    {
        private AddressableManager _addressableManager;
        private readonly Dictionary<SOKeys, ScriptableObject> _assetDictionary = new Dictionary<SOKeys, ScriptableObject>();
        private readonly Dictionary<SOKeys, AsyncOperationHandle> _assetHandlesDictionary = new Dictionary<SOKeys, AsyncOperationHandle>();
        public override bool IsCore => true;

        private SOAssetReferenceHolder _soAssetReferenceHolder;

        [Inject]
        private void Inject(AddressableManager addressableManager,
            SOAssetReferenceHolder soAssetReferenceHolder)
        {
            _addressableManager = addressableManager;
            _soAssetReferenceHolder = soAssetReferenceHolder;
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            SetReady();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            foreach (var handle in _assetHandlesDictionary)
            {
                if (handle.Value.IsValid())
                {
                    _addressableManager.ReleaseInstance(handle.Value);
                }
            }

            base.Dispose();
        }

        public T GetScriptableAsset<T>(SOKeys key) where T : ScriptableObject
        {
            _assetDictionary.TryGetValue(key, out var value);

            return (T)value;
        }

        public async UniTask<T> GetScriptableAsset<T>(SOKeys key, CancellationToken cancellationToken) where T : ScriptableObject
        {
            if (_assetDictionary.TryGetValue(key, out var value))
            {
                return value as T;
            }

            var reference = _soAssetReferenceHolder.GetAssetReference(key);
            var (asset, handle) = await _addressableManager.LoadAssetAsync(reference, cancellationToken);
            _assetDictionary.Add(key, asset);
            _assetHandlesDictionary.Add(key, handle);
            return asset as T;
        }
    }
}