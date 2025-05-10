using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Constants;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers;
using CerberusFramework.Core.Managers.Loading;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Extensions;
using CerberusFramework.Utilities.Logging;
using CerberusFramework.Utilities.MonoBehaviourUtilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace CerberusFramework.Core.Scenes
{
    public class PreloaderScene : IAsyncStartable
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(PreloaderScene));

        private PreloaderSceneController _preloaderSceneController;
        private LoadingManager _loadingManager;

        private IManager[] _managers;

        [Inject]
        public void Inject(
            PreloaderSceneController preloaderSceneController,
            LoadingManager loadingManager,
            IEnumerable<IManager> managers)
        {
            _preloaderSceneController = preloaderSceneController;
            _loadingManager = loadingManager;
            _managers = managers.ToArray();
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            if (_preloaderSceneController == null)
            {
                await UniTask.WaitUntil(() => _preloaderSceneController != null, cancellationToken: cancellation);
            }

            var gameStartTimestamp = TimeHelper.ScreenOnTime;

            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.backgroundLoadingPriority = ThreadPriority.High;

            var managersWaitWatch = PerformanceUtils.CFStopwatch("Managers wait");
            await _managers.WhenAllInTerminalState(cancellation);
            managersWaitWatch.Dispose();

            if (!_managers.AreAllCoreReady())
            {
                var notReadyManagers = _managers
                    .Where(m => m.IsCore && !m.ManagerState.IsReady())
                    .Select(m => m.GetType().Name)
                    .ToList();

                throw new InvalidOperationException(
                    "Core manager(s) are not ready. " + string.Join(", ", notReadyManagers)
                );
            }

            await _loadingManager.LoadMainScene();

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            Screen.sleepTimeout = SleepTimeout.SystemSetting;
            var loadingDuration = TimeHelper.ScreenOnTime - gameStartTimestamp;

            Logger.Debug(
                "EndPreloaderScene\n" +
                $"{EventConstants.ManagersLoadingDuration} {managersWaitWatch}\n" +
                $"{EventConstants.TotalOpenDuration} {loadingDuration}"
            );

            GlobalMessagePipe.GetPublisher<LoadingCompletedEvent>().Publish(new LoadingCompletedEvent());
        }
    }
}