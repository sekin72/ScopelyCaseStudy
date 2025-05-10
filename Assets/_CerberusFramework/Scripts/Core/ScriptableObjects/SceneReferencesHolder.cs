using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CerberusFramework.Core.Injection
{
    [CreateAssetMenu(fileName = "SceneReferencesHolder", menuName = "CerberusFramework/SceneReferencesHolder", order = 1)]
    public class SceneReferencesHolder : ScriptableObject
    {
        public AssetReference LevelSceneReference;
        public AssetReference LoadingSceneReference;
        public AssetReference MainMenuSceneReference;
        public AssetReference CFDemoSceneReference;
    }
}