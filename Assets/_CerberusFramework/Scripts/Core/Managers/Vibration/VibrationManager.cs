using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Lofelt.NiceVibrations;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.Data.Storages;
using CerberusFramework.Core.Managers.DeviceConfiguration;
using CerberusFramework.Utilities.Extensions;
using VContainer;
using static Lofelt.NiceVibrations.HapticPatterns;

namespace CerberusFramework.Core.Managers.Vibration
{
    public sealed class VibrationManager : Manager
    {
        private static VibrationManager _instance;
        private IVibrationConfiguration _vibrationConfiguration;

        private DataManager _dataManager;
        private DeviceConfigurationManager _deviceConfigurationManager;
        private SettingsStorage _storage;

        public override bool IsCore => false;
        public static bool CanUseHaptic { get; private set; } = true;

        [Inject]
        private void Inject(DataManager dataManager, DeviceConfigurationManager deviceConfigurationManager)
        {
            _dataManager = dataManager;
            _deviceConfigurationManager = deviceConfigurationManager;
        }

        protected override List<IManager> GetDependencies()
        {
            return new List<IManager>()
            {
                _dataManager,
                _deviceConfigurationManager
            };
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            _instance = this;

            LoadData();

            if (_deviceConfigurationManager.IsMobile || _deviceConfigurationManager.IsEditor)
            {
                CanUseHaptic = true;
                _vibrationConfiguration = new VibrationEnabledConfiguration();
                SetReady();
            }
            else
            {
                CanUseHaptic = false;
                _vibrationConfiguration = new VibrationDisabledConfiguration();
                SetState(ManagerState.Disabled);
            }

            _storage.IsVibrationActive = CanUseHaptic;
            Save();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            StopAllHaptics();

            base.Dispose();
        }

        public static void VibrateStatic(VibrationType vibrationType)
        {
            _instance?.Vibrate(vibrationType);
        }

        public void Vibrate(VibrationType vibrationType)
        {
            if (!IsReady() || !IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.Vibrate(vibrationType.GetPresetType());
        }

        public void SetVibrationActive(bool isActive)
        {
            if (!IsReady())
            {
                return;
            }

            _storage.IsVibrationActive = isActive;
            Save();

            if (isActive)
            {
                Vibrate(VibrationType.Selection);
            }
        }

        public void TriggerVibration(PresetType hapticType, float seconds, float vibrateAmountForSecond = 1)
        {
            if (!IsReady() || !IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.TriggerVibration(hapticType, seconds, vibrateAmountForSecond);
        }

        public void StopAllHaptics()
        {
            if (!IsVibrationActive())
            {
                return;
            }

            _vibrationConfiguration.ReleaseVibration();
            HapticController.Stop();
        }

        public bool IsVibrationActive()
        {
            return _storage.IsVibrationActive && CanUseHaptic;
        }

        #region Data

        protected override void LoadData()
        {
            _storage = _dataManager.Load<SettingsStorage>();
            if (_storage != null)
            {
                return;
            }

            _storage = new SettingsStorage();
            Save();
        }

        protected override void SaveData()
        {
            _dataManager.Save(_storage);
        }

        #endregion Data
    }
}