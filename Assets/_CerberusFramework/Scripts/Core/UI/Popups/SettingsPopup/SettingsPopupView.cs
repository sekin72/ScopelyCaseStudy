using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace CerberusFramework.Core.UI.Popups.Settings
{
    public class SettingsPopupView : PopupView
    {
        [SerializeField] protected GameObject SliderObject;
        [SerializeField] protected CFToggleButton SoundButton;
        [SerializeField] protected CFToggleButton VibrationButton;
        [SerializeField] protected GameObject VibrationObject;
        [SerializeField] protected Slider VolumeSlider;

        public event Action<bool> SoundToggled;

        public event Action<float> SoundVolumeChanged;

        public event Action<bool> VibrationToggled;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            SoundButton.OnValueChanged.AddListener(isOn => SoundToggled?.Invoke(isOn));
            VolumeSlider.onValueChanged.AddListener(value => SoundVolumeChanged?.Invoke(value));
            VibrationButton.OnValueChanged.AddListener(isOn => VibrationToggled?.Invoke(isOn));

            if (!VibrationManager.CanUseHaptic)
            {
                VibrationObject.SetActive(false);
            }
        }

        public override void Dispose()
        {
            SoundButton.OnValueChanged.RemoveAllListeners();
            VolumeSlider.onValueChanged.RemoveAllListeners();
            VibrationButton.OnValueChanged.RemoveAllListeners();

            base.Dispose();
        }

        public void SetSound(bool isActive)
        {
            SoundButton.IsOn = isActive;
            SliderObject.SetActive(isActive);
        }

        public void SetVolume(float value)
        {
            VolumeSlider.value = value;
        }

        public void SetVibration(bool isActive)
        {
            VibrationButton.IsOn = isActive;
        }
    }
}