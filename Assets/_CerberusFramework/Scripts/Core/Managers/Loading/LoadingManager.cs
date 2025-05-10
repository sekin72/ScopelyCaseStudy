using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Injection;
using CerberusFramework.Core.Managers.Asset;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using VContainer;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace CerberusFramework.Core.Managers.Loading
{
    public sealed class LoadingManager : Manager
    {
        private const string ScenePreloader = "PreloaderScene";
        private const string SceneLoading = "LoadingScene";

        public const string SceneLevelScene = "LevelScene";
        public const string SceneMain = "MainMenuScene";
        public const string SceneCFDemo = "CFDemoScene";

        private readonly Dictionary<(string, string), string> _loadingScenes = new Dictionary<(string, string), string>()
        {
            {
                (SceneMain, SceneLevelScene), SceneLoading
            },
            {
                (SceneMain, SceneCFDemo), SceneLoading
            },
            {
                (SceneLevelScene, SceneMain), SceneLoading
            },
            {
                (SceneCFDemo, SceneMain), SceneLoading
            },
            {
                (SceneMain, SceneMain), SceneLoading
            }
        };

        private readonly Dictionary<string, SceneController> _sceneControllers = new Dictionary<string, SceneController>();
        private readonly Dictionary<string, SceneInstance> _sceneInstances = new Dictionary<string, SceneInstance>();

        private readonly HashSet<string> _scenesBeingLoaded = new HashSet<string>();

        private AddressableManager _addressableManager;

        private bool _isGameScene;

        private bool _isLoadingAnyScene;
        private bool _isMainScene;

        private Scene _preloaderScene;

        private IPublisher<SceneChangeCompletedEvent> _sceneChangeCompletedPublisher;
        private IPublisher<SceneChangeStartingEvent> _sceneChangeStartingPublisher;

        private Dictionary<string, AssetReference> _sceneReferences = new Dictionary<string, AssetReference>();

        private SceneReferencesHolder _sceneReferencesHolder;

        public override bool IsCore => true;

        [Inject]
        private void Inject(AddressableManager addressableManager, SceneReferencesHolder sceneReferencesHolder)
        {
            _addressableManager = addressableManager;
            _sceneReferencesHolder = sceneReferencesHolder;
        }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            _sceneChangeStartingPublisher = GlobalMessagePipe.GetPublisher<SceneChangeStartingEvent>();
            _sceneChangeCompletedPublisher = GlobalMessagePipe.GetPublisher<SceneChangeCompletedEvent>();

            var currentScene = SceneManager.GetActiveScene();
            var sceneController = GetSceneController(currentScene);
            await sceneController.Initialize(DisposeToken);
            await sceneController.Activate(DisposeToken);

            if (currentScene.name == ScenePreloader)
            {
                _preloaderScene = currentScene;
            }

            _sceneReferences = new Dictionary<string, AssetReference>
            {
                {
                    SceneMain, _sceneReferencesHolder.MainMenuSceneReference
                },
                {
                    SceneLevelScene, _sceneReferencesHolder.LevelSceneReference
                },
                {
                    SceneLoading, _sceneReferencesHolder.LoadingSceneReference
                },
                {
                    SceneCFDemo, _sceneReferencesHolder.CFDemoSceneReference
                }
            };

            SetReady();
        }

        private async UniTask LoadScene(string toSceneName)
        {
            var cancellationToken = DisposeToken;

            if (_isLoadingAnyScene)
            {
                await UniTask.WaitWhile(() => _isLoadingAnyScene, cancellationToken: cancellationToken);
            }

            _isLoadingAnyScene = true;

            var fromScene = SceneManager.GetActiveScene();
            var fromSceneName = fromScene.name;
            var fromSceneController = GetSceneController(fromScene);

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            CFButton.IsInputLocked.Increase("SceneLoading");

            _sceneChangeStartingPublisher.Publish(new SceneChangeStartingEvent(fromSceneName, toSceneName));
            fromSceneController.SceneWillBeDeactivated();

            LoadingSceneController loadingSceneController;
            if (fromSceneController is LoadingSceneController controller)
            {
                loadingSceneController = controller;
            }
            else
            {
                loadingSceneController = await ShowLoadingScene(fromSceneName, toSceneName, cancellationToken);
                if (loadingSceneController != null)
                {
                    await UnloadScene(fromSceneName, cancellationToken);
                }
            }

            Application.backgroundLoadingPriority = ThreadPriority.High;

            var toSceneController = await ActivateScene(toSceneName, cancellationToken);

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            if (loadingSceneController != null)
            {
                await loadingSceneController.HideEffect(cancellationToken);
            }

            toSceneController.SceneVisible();

            if (loadingSceneController != null)
            {
                await UnloadScene(loadingSceneController.gameObject.scene.name, cancellationToken);
            }
            else
            {
                await UnloadScene(fromSceneName, cancellationToken);
            }

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            _sceneChangeCompletedPublisher.Publish(new SceneChangeCompletedEvent(fromSceneName, toSceneName));

            CFButton.IsInputLocked.Decrease("SceneLoading");
            _isLoadingAnyScene = false;
        }

        private async UniTask<LoadingSceneController> ShowLoadingScene(string fromSceneName, string toSceneName,
            CancellationToken cancellationToken)
        {
            if (!_loadingScenes.TryGetValue((fromSceneName, toSceneName), out var loadingSceneName))
            {
                return null;
            }

            var loadingSceneController =
                (LoadingSceneController)await ActivateScene(loadingSceneName, cancellationToken);

            await loadingSceneController.ShowEffect(cancellationToken);

            return loadingSceneController;
        }

        public async UniTask<SceneInstance> GetScene(string sceneName, CancellationToken cancellationToken)
        {
            if (_scenesBeingLoaded.Contains(sceneName))
            {
                await UniTask.WaitWhile(
                    () => _scenesBeingLoaded.Contains(sceneName), cancellationToken: cancellationToken
                );
            }

            _sceneInstances.TryGetValue(sceneName, out var sceneInstance);
            var scene = sceneInstance.Scene;

            if (sceneName == ScenePreloader)
            {
                scene = _preloaderScene;
            }

            _sceneInstances.TryGetValue(sceneName, out sceneInstance);

            if (sceneInstance.Scene != default(SceneInstance).Scene)
            {
                return sceneInstance;
            }

            if (!scene.IsValid() || !scene.isLoaded)
            {
                try
                {
                    _scenesBeingLoaded.Add(sceneName);

                    var loadedInstance = await _addressableManager.LoadSceneAdditive(
                        _sceneReferences[sceneName], cancellationToken
                    );
                    _sceneInstances.Add(sceneName, loadedInstance);
                }
                finally
                {
                    _scenesBeingLoaded.Remove(sceneName);
                }

                _sceneInstances.TryGetValue(sceneName, out sceneInstance);
            }

            return sceneInstance;
        }

        private async UniTask<SceneController> ActivateScene(string sceneName, CancellationToken cancellationToken)
        {
            var sceneInstance = await GetScene(sceneName, cancellationToken);

            await sceneInstance.ActivateAsync().WithCancellation(cancellationToken);

            var sceneController = GetSceneController(sceneInstance.Scene);
            SceneManager.SetActiveScene(sceneController.gameObject.scene);
            await sceneController.Initialize(cancellationToken);
            await sceneController.Activate(cancellationToken);

            return sceneController;
        }

        private async UniTask UnloadScene(string sceneName, CancellationToken cancellationToken)
        {
            _sceneInstances.TryGetValue(sceneName, out var sceneInstance);
            var scene = sceneInstance.Scene;

            if (sceneName == ScenePreloader)
            {
                scene = _preloaderScene;
            }

            var sceneController = GetSceneController(scene);
            await sceneController.Deactivate(cancellationToken);
            sceneController.SceneInvisible();

            if (!sceneController.KeepSceneLoaded())
            {
                if (_sceneInstances.ContainsKey(sceneName))
                {
                    await _addressableManager.UnloadScene(sceneInstance, cancellationToken);
                }
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await SceneManager.UnloadSceneAsync(scene);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                _sceneInstances.Remove(sceneName);
                _sceneControllers.Remove(sceneName);

                GC.Collect();
                await Resources.UnloadUnusedAssets().WithCancellation(cancellationToken);
            }
        }

        private SceneController GetSceneController(Scene scene)
        {
            if (_sceneControllers.TryGetValue(scene.name, out var sceneController))
            {
                return sceneController;
            }

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                if (!rootGameObject.TryGetComponent(out sceneController))
                {
                    continue;
                }

                _sceneControllers.Add(scene.name, sceneController);
                return sceneController;
            }

            if (string.IsNullOrEmpty(scene.name))
            {
                throw new InvalidOperationException(
                    "The scene provided does not have a SceneController. Did you forget to open an existing scene?"
                );
            }

            throw new InvalidOperationException(
                $"{scene.name} does not have a SceneController. All scenes must have one."
            );
        }

        public async UniTask LoadLevelScene()
        {
            _isMainScene = false;
            await LoadScene(SceneLevelScene);
            _isGameScene = true;
        }

        public async UniTask LoadMainScene()
        {
            _isGameScene = false;
            await LoadScene(SceneMain);
            _isMainScene = true;
        }

        public async UniTask LoadCFDemoScene()
        {
            _isGameScene = false;
            _isMainScene = false;
            await LoadScene(SceneCFDemo);
        }

        public bool IsMainScene()
        {
            return _isMainScene;
        }

        public bool IsGameScene()
        {
            return _isGameScene;
        }
    }
}