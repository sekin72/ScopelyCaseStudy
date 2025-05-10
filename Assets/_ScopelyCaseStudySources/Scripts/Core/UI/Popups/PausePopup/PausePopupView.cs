using System;
using System.Threading;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause
{
    public class PausePopupView : PopupView
    {
        [SerializeField] protected CFToggleButton SoundButton;
        [SerializeField] protected Slider VolumeSlider;
        [SerializeField] protected GameObject SliderObject;
        [SerializeField] protected CFToggleButton VibrationButton;
        [SerializeField] protected GameObject VibrationObject;
        [SerializeField] protected CFButton SaveButton;
        [SerializeField] protected CFButton LoadButton;
        [SerializeField] protected CFButton MMButton;

        public event Action<bool> SoundToggled;

        public event Action<float> SoundVolumeChanged;

        public event Action<bool> VibrationToggled;

        public event Action SaveButtonClicked;

        public event Action LoadButtonClicked;

        public event Action MMButtonClicked;

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

            SaveButton.onClick.AddListener(() => SaveButtonClicked?.Invoke());
            LoadButton.onClick.AddListener(() => LoadButtonClicked?.Invoke());
            MMButton.onClick.AddListener(() => MMButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            SoundButton.OnValueChanged.RemoveAllListeners();
            VibrationButton.OnValueChanged.RemoveAllListeners();
            VolumeSlider.onValueChanged.RemoveAllListeners();
            MMButton.onClick.RemoveAllListeners();
            SaveButton.onClick.RemoveAllListeners();
            LoadButton.onClick.RemoveAllListeners();

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
            if (VibrationButton != null)
            {
                VibrationButton.IsOn = isActive;
            }
        }
    }
}