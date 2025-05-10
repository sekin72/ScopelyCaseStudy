using UnityEngine.AddressableAssets;

namespace CerberusFramework.Utilities.Extensions
{
    public static class AssetReferenceExtensions
    {
        public static bool IsValidReference(this AssetReference assetReference)
        {
            return assetReference?.RuntimeKey != null && assetReference.RuntimeKeyIsValid();
        }
    }
}