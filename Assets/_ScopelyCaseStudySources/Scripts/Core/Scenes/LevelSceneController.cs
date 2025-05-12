using System;
using System.Threading;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.Loading;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Core.UI.Popups.Loading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using RTS_Cam;
using ScopelyCaseStudy.Core.Gameplay;
using ScopelyCaseStudy.Core.Gameplay.UI;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Scenes
{
    public class LevelSceneController : SceneController
    {
        public event Action Tick;

        public event Action LateTick;

        private IObjectResolver _resolver;
        private PopupManager _popupManager;
        private LoadingManager _loadingManager;
        private PoolWarmUpManager _poolWarmUpManager;

        private GameSession _session;
        private IDisposable _messageSubscription;
        private CancellationTokenSource _oldCancellationTokenSource;
        private CancellationToken _originalCancellationToken;

        [SerializeField] private GameObject _light;
        [SerializeField] private CFButton _pauseButton;

        [SerializeField] private LevelScenePanel _levelScenePanel;

        public RTS_Camera RTSCamera;

        [Inject]
        public void Inject(
            IObjectResolver objectResolver,
            PopupManager popupManager,
            LoadingManager loadingManager,
            PoolWarmUpManager levelWarmUpManager)
        {
            _resolver = objectResolver;
            _popupManager = popupManager;
            _loadingManager = loadingManager;
            _poolWarmUpManager = levelWarmUpManager;
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            _resolver.Inject(UIContainer);

            _originalCancellationToken = cancellationToken;

            await base.Activate(cancellationToken);

            await LoadLevel();
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _levelScenePanel.Dispose();

            _session?.Dispose();
            _session = null;

            _messageSubscription?.Dispose();
            _messageSubscription = null;

            return base.Deactivate(cancellationToken);
        }

        public override void SceneVisible()
        {
            _light.SetActive(true);
            base.SceneVisible();
        }

        public override void SceneInvisible()
        {
            base.SceneInvisible();

            _light.SetActive(false);
            _popupManager.ClearAll();
        }

        private async UniTask LoadLevel()
        {
            _pauseButton.onClick.RemoveAllListeners();

            if (_oldCancellationTokenSource != null)
            {
                _oldCancellationTokenSource.Cancel();
                _oldCancellationTokenSource.Dispose();
            }

            _session?.Dispose();
            _session = null;

            await LoadLevelInternal();
        }

        private async UniTask LoadLevelInternal()
        {
            _oldCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_originalCancellationToken);
            var linkedCancellationToken = _oldCancellationTokenSource.Token;

            if (!_poolWarmUpManager.LevelWarmUpCompleted)
            {
                await _poolWarmUpManager.StartRemainingJobsForceful(linkedCancellationToken);
            }

            _session = _resolver.Resolve<GameSession>();

            await _popupManager.Open<LoadingPopup, LoadingPopupData, LoadingPopupView>(new LoadingPopupData(), PopupShowActions.CloseAll, linkedCancellationToken);

            await _session.Initialize(this);

            _levelScenePanel.Initialize(_session, SceneCamera);

            _pauseButton.onClick.AddListener(() => OpenPausePopup(linkedCancellationToken));

            var popup = _popupManager.GetPopup<LoadingPopup>();
            await _popupManager.Close(popup, linkedCancellationToken);

            _session.Activate();
        }

        private void OpenPausePopup(CancellationToken cancellationToken)
        {
            _session.PauseGame();
            _popupManager.Open<PausePopup, PausePopupData, PausePopupView>(new PausePopupData(null, RestartLevel, ReturnToMainScene, _session.ResumeGame), PopupShowActions.CloseAll,
                cancellationToken).Forget();
        }

        public void ReturnToMainScene()
        {
            _levelScenePanel.Dispose();
            _session.Dispose();
            _session = null;

            _loadingManager.LoadMainScene().Forget();
        }

        public void RestartLevel()
        {
            var oldStorage = _session.GameSessionSaveStorage;
            _session.SetGameSessionStorage(new GameSessionSaveStorage
            {
                HighScore = oldStorage.HighScore,
                CurrentLevel = oldStorage.CurrentLevel,
                LevelRandomSeed = oldStorage.LevelRandomSeed,
            });

            _levelScenePanel.Dispose();
            LoadLevel().Forget();
        }

        [UsedImplicitly]
        private void Update()
        {
            Tick?.Invoke();

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartLevel();
            }
        }

        [UsedImplicitly]
        private void LateUpdate()
        {
            LateTick?.Invoke();
        }
    }
}