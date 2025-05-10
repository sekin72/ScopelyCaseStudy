using JetBrains.Annotations;
using CerberusFramework.Utilities.Logging;
using UnityEngine;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class TimeHelper : MonoBehaviour
    {
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(TimeHelper));
        private static bool _isActive;

        private static bool _isPaused;
        private static float _lastScreenOnTimestamp;

        private static float _timeUnfocusedDuringSplashTimeStamp;

        private bool _isApplicationQuiting;

        static TimeHelper()
        {
            Application.focusChanged += Application_focusChanged;
        }

        public static float AwakeTime { get; private set; }
        public static float ScreenOnTime { get; private set; }
        public static float TimePassedSinceResume { get; private set; }

        public static bool IsApplicationPausedStatic { get; private set; }

        public static float TimeUnfocusedDuringSplashDuration { get; private set; }

        public static void Initialize()
        {
            if (_isActive)
            {
                return;
            }

            var go = new GameObject("TimeHelper", typeof(TimeHelper));
            DontDestroyOnLoad(go);
        }

        [UsedImplicitly]
        public void Awake()
        {
            ScreenOnTime = 0f;
            TimeUnfocusedDuringSplashDuration = 0f;
            _lastScreenOnTimestamp = Time.realtimeSinceStartup;
            AwakeTime = Time.realtimeSinceStartup;

            Application.focusChanged -= Application_focusChanged;

            _isActive = true;
        }

        [UsedImplicitly]
        public void OnDestroy()
        {
            if (_isApplicationQuiting)
            {
                return;
            }

            Logger.Error($"{nameof(TimeHelper)} is destroyed before application quit.");
            _isActive = false;
        }

        [UsedImplicitly]
        public void Update()
        {
            if (_isPaused)
            {
                return;
            }

            var now = Time.realtimeSinceStartup;
            var delta = now - _lastScreenOnTimestamp;
            ScreenOnTime += delta;
            TimePassedSinceResume += delta;
            _lastScreenOnTimestamp = now;
        }

        [UsedImplicitly]
        public void OnApplicationPause(bool isPaused)
        {
            _isPaused = isPaused;
            _lastScreenOnTimestamp = Time.realtimeSinceStartup;
            TimePassedSinceResume = 0f;
        }

        [UsedImplicitly]
        public void OnApplicationQuit()
        {
            _isApplicationQuiting = true;
        }

        private static void Application_focusChanged(bool obj)
        {
            if (!obj)
            {
                _timeUnfocusedDuringSplashTimeStamp = Time.realtimeSinceStartup;
                IsApplicationPausedStatic = true;
            }
            else
            {
                TimeUnfocusedDuringSplashDuration += Time.realtimeSinceStartup - _timeUnfocusedDuringSplashTimeStamp;
            }
        }
    }
}