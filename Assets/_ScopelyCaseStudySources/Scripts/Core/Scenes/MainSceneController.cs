using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
using ScopelyCaseStudy.Core.Gameplay.GameData;
using TMPro;
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
        private AssetManager _assetManager;

        [SerializeField] private TMP_Dropdown _levelsDropdown;
        [SerializeField] private CFButton _newGameButton;
        [SerializeField] private CFButton _loadButton;
        [SerializeField] private CFButton _settingsButton;
        [SerializeField] private CFButton _cfDemoButton;

        private int _selectedLevel;

        [Inject]
        public void Inject(
            PopupManager popupManager,
            LoadingManager loadingManager,
            SoundManager soundManager,
            DataManager dataManager,
            AssetManager assetManager)
        {
            PopupManager = popupManager;
            _loadingManager = loadingManager;
            _soundManager = soundManager;
            _dataManager = dataManager;
            _assetManager = assetManager;
        }

        public override async UniTask Activate(CancellationToken cancellationToken)
        {
            _loadButton.interactable = _dataManager.Load<GameSessionSaveStorage>() is { GameplayFinished: false };

            _newGameButton.onClick.AddListener(OnNewGameButtonClick);
            _loadButton.onClick.AddListener(OnLoadButtonClick);
            _settingsButton.onClick.AddListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.AddListener(OnCFDemoButtonClicked);
            _levelsDropdown.onValueChanged.AddListener(OnSelectedLevelChanged);

            var levelDataHolder = _assetManager.GetScriptableAsset<LevelDataHolder>(SOKeys.LevelDataHolder);
            if (levelDataHolder == null)
            {
                await _assetManager.GetScriptableAsset<LevelDataHolder>(SOKeys.LevelDataHolder, cancellationToken)
                    .ContinueWith(col => levelDataHolder = col);
            }

            _levelsDropdown.options = new List<TMP_Dropdown.OptionData>();
            for (var i = 0; i < levelDataHolder.LevelData.Count; i++)
            {
                _levelsDropdown.options.Add(new TMP_Dropdown.OptionData($"Level {i + 1}"));
            }

            RefreshLevelsDropdown(0);

            await base.Activate(cancellationToken);
        }

        public override UniTask Deactivate(CancellationToken cancellationToken)
        {
            _soundManager.StopAll();

            _newGameButton.onClick.RemoveListener(OnNewGameButtonClick);
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            _cfDemoButton.onClick.RemoveListener(OnCFDemoButtonClicked);
            _levelsDropdown.onValueChanged.RemoveListener(OnSelectedLevelChanged);

            return base.Deactivate(cancellationToken);
        }

        private void OnNewGameButtonClick()
        {
            _dataManager.Save(new GameSessionSaveStorage
            {
                GameplayFinished = false,
                LevelRandomSeed = Mathf.Abs((int)DateTime.Now.Ticks),
                CurrentLevel = _selectedLevel,
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

        private void RefreshLevelsDropdown(int index)
        {
            _levelsDropdown.SetValueWithoutNotify(index);
            _levelsDropdown.value = index;

            _levelsDropdown.RefreshShownValue();
        }

        private void OnSelectedLevelChanged(int index)
        {
            _selectedLevel = index;

            RefreshLevelsDropdown(index);
        }
    }
}