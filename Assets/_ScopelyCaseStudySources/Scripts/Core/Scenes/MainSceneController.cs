using System;
using System.Threading;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.Loading;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Core.UI.Popups.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace ScopelyCaseStudy.Core.Scenes
{
    public class MainSceneController : SceneController
    {
        protected PopupManager PopupManager;
        private LoadingManager _loadingManager;
        private SoundManager _soundManager;
        private DataManager _dataManager;

        [SerializeField] private CFButton _newGameButton;
        [SerializeField] private CFButton _loadButton;
        [SerializeField] private CFButton _settingsButton;
        [SerializeField] private CFButton _cfDemoButton;

        [Inject]
        public void Inject(
            PopupManager popupManager,
            LoadingManager loadingManager,
            SoundManager soundManager,
            DataManager dataManager)
        {
            PopupManager = popupManager;
            _loadingManager = loadingManager;
            _soundManager = soundManager;
            _dataManager = dataManager;
        }

        public override UniTask Activate(CancellationToken cancellationToken)
        {
            _loadButton.interactable = _dataManager.Load<GameSessionSaveStorage>() is { GameplayFinished: false };

            _newGameButton.onClick.AddListener(OnNewGameButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.AddListener(OnCFDemoButtonClicked);

            return base.Activate(cancellationToken);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _soundManager.StopAll();

            _newGameButton.onClick.RemoveListener(OnNewGameButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.RemoveListener(OnCFDemoButtonClicked);

            return base.Deactivate(cancellationToken);
        }

        private void OnNewGameButtonClick()
        {
            _dataManager.Save(new GameSessionSaveStorage
            {
                GameplayFinished = false,
                LevelRandomSeed = Mathf.Abs((int)DateTime.Now.Ticks)
            });

            _loadingManager.LoadLevelScene().Forget();
        }

        private void OnLoadButtonClick()
        {
            _loadingManager.LoadLevelScene().Forget();
        }

        private void OnSettingsButtonClick()
        {
            PopupManager.Open<SettingsPopup, SettingsPopupData, SettingsPopupView>(new SettingsPopupData(), PopupShowActions.CloseAll, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnCFDemoButtonClicked()
        {
            _loadingManager.LoadCFDemoScene().Forget();
        }
    }
}