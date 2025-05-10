using System;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Config;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.Loading;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.Scenes;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Logging;
using CFGameClient.Core.Gameplay;
using CFGameClient.Core.Gameplay.Systems;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win;
using ScopelyCaseStudy.Core.Scenes;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Gameplay
{
    public class GameSession : IGameSession
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(GameSession));
        private readonly IObjectResolver _resolver;
        private IDisposable _messageSubscription;

        private List<ITickable> _tickables;
        private List<ILateTickable> _lateTickables;

        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public LockBin InputDisabled { get; private set; }

        private readonly DataManager _dataManager;
        private readonly PopupManager _popupManager;
        private readonly SoundManager _soundManager;
        private readonly VibrationManager _vibrationManager;
        private readonly AssetManager _assetManager;

        private List<IGameSystem> _gameSystems;
        private Dictionary<Type, IGameSystem> _gameSystemsDictionary;

        private LevelSceneController _levelSceneController;

        public GameSessionSaveStorage GameSessionSaveStorage { get; private set; }
        public GameSettings GameSettings { get; private set; }

        private bool _deactivated;
        private bool _disposed;

        [Inject]
        public GameSession(
            IObjectResolver resolver,
            DataManager dataManager,
            LoadingManager loadingManager,
            PopupManager popupManager,
            SoundManager soundManager,
            VibrationManager vibrationManager,
            AssetManager assetManager)
        {
            _resolver = resolver;
            _dataManager = dataManager;
            _popupManager = popupManager;
            _soundManager = soundManager;
            _vibrationManager = vibrationManager;
            _assetManager = assetManager;
        }

        public async UniTask Initialize(SceneController levelSceneController)
        {
            _disposed = false;
            _deactivated = false;

            _levelSceneController = (LevelSceneController)levelSceneController;

            Application.targetFrameRate = 60;

            CancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = CancellationTokenSource.Token;

            _soundManager.StopAll();

            _gameSystems = ListPool<IGameSystem>.Get();
            _gameSystemsDictionary = DictionaryPool<Type, IGameSystem>.Get();

            var bagBuilder = DisposableBag.CreateBuilder();
            _messageSubscription = bagBuilder.Build();

            InputDisabled = new LockBin();
            _tickables = ListPool<ITickable>.Get();
            _lateTickables = ListPool<ILateTickable>.Get();

            GameSessionSaveStorage = _dataManager.Load<GameSessionSaveStorage>();

            var tasks = new List<UniTask>();

            GameSettings = _assetManager.GetScriptableAsset<GameSettings>(SOKeys.GameSettings);

            if (GameSettings == null)
            {
                tasks.Add(_assetManager.GetScriptableAsset<GameSettings>(SOKeys.GameSettings, cancellationToken)
                    .ContinueWith(gameSettings => GameSettings = gameSettings));
            }

            var systemsCollection = _assetManager.GetScriptableAsset<SystemsCollection>(SOKeys.SystemsCollection);

            if (systemsCollection == null)
            {
                tasks.Add(_assetManager.GetScriptableAsset<SystemsCollection>(SOKeys.SystemsCollection, cancellationToken)
                    .ContinueWith(col => systemsCollection = col));
            }

            if (tasks.Count > 0)
            {
                await UniTask.WhenAll(tasks);
            }

            RegisterSystems(systemsCollection);

            foreach (var system in _gameSystems)
            {
                await system.Initialize(this, cancellationToken);
            }
        }

        public void Activate()
        {
            foreach (var system in _gameSystems)
            {
                system.Activate();
            }

            RegisterTicks();
        }

        private void Deactivate()
        {
            if (_deactivated)
            {
                return;
            }

            _deactivated = true;

            if (_tickables.Count > 0)
            {
                _levelSceneController.Tick -= Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _levelSceneController.LateTick -= LateTick;
            }

            ListPool<ITickable>.Release(_tickables);
            ListPool<ILateTickable>.Release(_lateTickables);

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Deactivate();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Deactivate();

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;

            _disposed = true;

            _messageSubscription?.Dispose();

            for (var i = _gameSystems.Count - 1; i >= 0; i--)
            {
                _gameSystems[i]?.Dispose();
            }

            ListPool<IGameSystem>.Release(_gameSystems);
            DictionaryPool<Type, IGameSystem>.Release(_gameSystemsDictionary);
        }

        private void Tick()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }

        private void LateTick()
        {
            foreach (var lateTickable in _lateTickables)
            {
                lateTickable.LateTick();
            }
        }

        public void LevelFinished(bool success)
        {
            if (_deactivated)
            {
                return;
            }

            Deactivate();

            GameSessionSaveStorage.GameplayFinished = true;
            SaveGameSessionStorage();

            if (!success)
            {
                _popupManager.Open<FailPopup, FailPopupData, FailPopupView>(
                    new FailPopupData(
                        _levelSceneController),
                    PopupShowActions.CloseAll,
                    CancellationTokenSource.Token).Forget();
                return;
            }

            _soundManager.PlayOneShot(SoundKeys.LevelCompleted);
            _vibrationManager.Vibrate(VibrationType.Success);
            _popupManager.Open<WinPopup, WinPopupData, WinPopupView>(
                new WinPopupData(
                    _levelSceneController),
                PopupShowActions.CloseAll,
                CancellationTokenSource.Token).Forget();
        }

        private void RegisterTicks()
        {
            if (_tickables.Count > 0)
            {
                _levelSceneController.Tick += Tick;
            }

            if (_lateTickables.Count > 0)
            {
                _levelSceneController.LateTick += LateTick;
            }
        }

        private void RegisterSystems(SystemsCollection systemsCollection)
        {
            foreach (var system in systemsCollection.Systems)
            {
                _gameSystems.Add(system);
                _resolver.Inject(system);
                _gameSystemsDictionary.Add(system.RegisterType, system);

                if (system is ITickable tickable)
                {
                    _tickables.Add(tickable);
                }

                if (system is ILateTickable lateTickable)
                {
                    _lateTickables.Add(lateTickable);
                }
            }
        }

        public T GetSystem<T>() where T : IGameSystem
        {
            _gameSystemsDictionary.TryGetValue(typeof(T), out var system);
            return (T)system;
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void SaveGameSessionStorage()
        {
            _dataManager.Save(GameSessionSaveStorage);
        }
    }
}