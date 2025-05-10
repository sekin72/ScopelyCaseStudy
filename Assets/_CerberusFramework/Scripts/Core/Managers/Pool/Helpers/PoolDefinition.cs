using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CerberusFramework.Core.Managers.Pool
{
    [Serializable]
    public class PoolDefinition
    {
        public PoolKeys Key;
        public int DefaultCapacity;
        public bool PreWarm;
        public AssetReferenceT<GameObject> AssetReference;

        public PoolDefinition(
            PoolKeys key,
            int defaultCapacity,
            bool preWarm,
            AssetReferenceT<GameObject> assetReference)
        {
            Key = key;
            DefaultCapacity = defaultCapacity;
            PreWarm = preWarm;
            AssetReference = assetReference;
        }
    }
}