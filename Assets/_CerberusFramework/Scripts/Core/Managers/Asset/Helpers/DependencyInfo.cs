using System;
using UnityEngine.AddressableAssets;

namespace CerberusFramework.Core.Managers.Asset.Helpers
{
    public class DependencyInfo : IComparable<DependencyInfo>
    {
        private readonly string _assetReferenceRuntimeKeyString;
        public readonly AssetReference AssetReference;
        public DependencyDownloadStatus DependencyDownloadStatus;
        public string DisplayNameForDebugging;
        public long SizeAtAppStart;
        public long SizeUpdated;

        public DependencyInfo(
            AssetReference assetReference,
            string displayNameForDebugging,
            long sizeAtAppStart,
            long sizeUpdated,
            DependencyDownloadStatus dependencyDownloadStatus
        )
        {
            AssetReference = assetReference;
            SizeAtAppStart = sizeAtAppStart;
            SizeUpdated = sizeUpdated;
            DependencyDownloadStatus = dependencyDownloadStatus;

            _assetReferenceRuntimeKeyString = assetReference.RuntimeKey.ToString();
            SetAsyncOperationLoadProgress();

            DisplayNameForDebugging = _assetReferenceRuntimeKeyString;
            SetDisplayName(displayNameForDebugging);
        }

        public AsyncOperationLoadProgress AsyncOperationLoadProgress { get; private set; }

        public int CompareTo(DependencyInfo other)
        {
            return AssetReference == other.AssetReference ? 0 : 1;
        }

        public void SetAsyncOperationLoadProgress()
        {
            AsyncOperationLoadProgress = new AsyncOperationLoadProgress(_assetReferenceRuntimeKeyString);
        }

        public void SetDisplayName(string displayNameForDebugging)
        {
            if (displayNameForDebugging.Contains("Clone"))
            {
                displayNameForDebugging = displayNameForDebugging.Replace("(Clone)", "");
            }

            if (DisplayNameForDebugging.Equals(_assetReferenceRuntimeKeyString))
            {
                DisplayNameForDebugging = displayNameForDebugging;
            }
        }
    }
}