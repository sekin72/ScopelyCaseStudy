using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Managers.Vibration;
using VContainer;

namespace CerberusFramework.Core.UI.Popups.Settings
{
    public class SettingsPopup : Popup<SettingsPopupData, SettingsPopupView>
    {
        private SoundManager _audioManager;
        private VibrationManager _vibrationManager;

        [Inject]
        public void Inject(SoundManager audioManager, VibrationManager vibrationManager)
        {
            _audioManager = audioManager;
            _vibrationManager = vibrationManager;
        }

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);
            View.SoundToggled += OnSoundToggled;
            View.SoundVolumeChanged += OnSoundVolumeChanged;
            View.VibrationToggled += OnVibrationToggled;

            View.SetSound(_audioManager.IsSoundActive());
            View.SetVolume(_audioManager.GetVolume());
            View.SetVibration(_vibrationManager.IsVibrationActive());
        }

        protected override void Dispose()
        {
            View.SoundToggled -= OnSoundToggled;
            View.SoundVolumeChanged -= OnSoundVolumeChanged;
            View.VibrationToggled -= OnVibrationToggled;

            base.Dispose();
        }

        private void OnSoundToggled(bool isOn)
        {
            _audioManager.SetSoundActive(isOn);
            View.SetSound(_audioManager.IsSoundActive());
            View.SetVolume(_audioManager.GetVolume());
        }

        private void OnSoundVolumeChanged(float value)
        {
            _audioManager.SetSoundVolume(value);
            View.SetVolume(_audioManager.GetVolume());
        }

        private void OnVibrationToggled(bool isOn)
        {
            _vibrationManager.SetVibrationActive(isOn);
            View.SetVibration(_vibrationManager.IsVibrationActive());
        }
    }
}