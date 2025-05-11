using System.Threading;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause
{
    public class PausePopup : Popup<PausePopupData, PausePopupView>
    {
        private SoundManager _audioManager;
        private VibrationManager _vibrationManager;

        private bool _isReadingInProgress;

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
            View.SaveButtonClicked += OnSaveClicked;
            View.LoadButtonClicked += OnLoadClicked;
            View.MMButtonClicked += OnMMClicked;

            View.SetSound(_audioManager.IsSoundActive());
            View.SetVolume(_audioManager.GetVolume());
            View.SetVibration(_vibrationManager.IsVibrationActive());
        }

        protected override void Dispose()
        {
            if (Data.IsDisposed)
            {
                Logger.Warn($"Trying to dispose {View.name}, but it's already disposed");
                return;
            }

            View.SoundToggled -= OnSoundToggled;
            View.SoundVolumeChanged -= OnSoundVolumeChanged;
            View.VibrationToggled -= OnVibrationToggled;
            View.SaveButtonClicked -= OnSaveClicked;
            View.LoadButtonClicked -= OnLoadClicked;
            View.MMButtonClicked -= OnMMClicked;

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

        private void OnMMClicked()
        {
            ClosePopup();
            Data.OnMMButtonClicked?.Invoke();
        }

        private void OnSaveClicked()
        {
            ClosePopup();
            Data.OnSaveButtonClicked?.Invoke();
        }

        private void OnLoadClicked()
        {
            ClosePopup();
            Data.OnLoadButtonClicked?.Invoke();
        }
    }
}