using System;
using System.Collections.Generic;
using CerberusFramework.Core.Managers.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CerberusFramework.Config
{
    [CreateAssetMenu(fileName = "SOAssetReferenceHolder", menuName = "CerberusFramework/ScriptableObjects/SOAssetReferenceHolder", order = 1)]
    public class SOAssetReferenceHolder : ScriptableObject
    {
        [SerializeField] private readonly Dictionary<SOKeys, AssetReferenceT<ScriptableObject>> _poolReferenceDict = new Dictionary<SOKeys, AssetReferenceT<ScriptableObject>>();

        public List<SOAssetReferenceDefinition> SOAssetReferenceDefinitions;

        public AssetReferenceT<ScriptableObject> GetAssetReference(SOKeys key)
        {
            if (_poolReferenceDict.Count == 0)
            {
                Sort();
            }

            if (_poolReferenceDict.TryGetValue(key, out var value))
            {
                return value;
            }

            return null;
        }

        public void Sort()
        {
            SOAssetReferenceDefinitions.Sort((a, b) => a.PoolKey.CompareTo(b.PoolKey));
            _poolReferenceDict.Clear();
            foreach (var reference in SOAssetReferenceDefinitions)
            {
                _poolReferenceDict.Add(reference.PoolKey, reference.AssetReference);
            }
        }
    }

    [Serializable]
    public class SOAssetReferenceDefinition
    {
        public SOKeys PoolKey;
        public AssetReferenceT<ScriptableObject> AssetReference;

        public SOAssetReferenceDefinition(SOKeys poolKey, AssetReferenceT<ScriptableObject> assetReference)
        {
            PoolKey = poolKey;
            AssetReference = assetReference;
        }
    }
}