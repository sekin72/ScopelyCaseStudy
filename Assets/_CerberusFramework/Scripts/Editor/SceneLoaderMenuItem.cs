#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using VContainer.Unity;

namespace CerberusFramework.Editor
{
    public class SceneLoaderMenuItem
    {
        //private const string StartFromPreloaderName = "StartFromPreloader";

        //public static bool StartFromPreloaderEnabled
        //{
        //    get { return EditorPrefs.GetBool(StartFromPreloaderName, true); }
        //    set
        //    {
        //        EditorPrefs.SetBool(StartFromPreloaderName, value);
        //    }
        //}

        //[MenuItem("CerberusFramework/Scenes/StartFromPreloader", false, 0)]
        //private static void ToggleAction()
        //{
        //    StartFromPreloaderEnabled = !StartFromPreloaderEnabled;
        //}

        //[MenuItem("CerberusFramework/Scenes/StartFromPreloader", true)]
        //private static bool ToggleActionValidate()
        //{
        //    Menu.SetChecked("CerberusFramework/Scenes/StartFromPreloader", StartFromPreloaderEnabled);
        //    return true;
        //}

        [MenuItem("CerberusFramework/Scenes/PreloaderScene", false, 50)]
        public static void LoadPreloaderScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_CerberusFramework/Scenes/PreloaderScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/LoadingScene", false, 51)]
        public static void LoadLoadingScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_CerberusFramework/Scenes/LoadingScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/MainMenuScene", false, 52)]
        public static void LoadMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_ScopelyCaseStudySources/Scenes/MainMenuScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/LevelScene", false, 53)]
        public static void LoadLevelScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_ScopelyCaseStudySources/Scenes/LevelScene.unity", OpenSceneMode.Single);
            }
        }

        [MenuItem("CerberusFramework/Scenes/CFDemoScene", false, 54)]
        public static void LoadCFDemoScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene("Assets/_ScopelyCaseStudySources/Scenes/CFDemoScene.unity", OpenSceneMode.Single);
            }
        }
    }
}
#endif