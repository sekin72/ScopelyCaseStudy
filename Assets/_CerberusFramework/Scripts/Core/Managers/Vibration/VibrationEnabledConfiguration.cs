using System;
using DG.Tweening;
using static Lofelt.NiceVibrations.HapticPatterns;
using Random = UnityEngine.Random;

namespace CerberusFramework.Core.Managers.Vibration
{
    public class VibrationEnabledConfiguration : IVibrationConfiguration
    {
        private Tween _delayTween;
        private Tween _triggerTween;

        public void Vibrate(PresetType hapticType)
        {
            PlayPreset(hapticType);
        }

        public void TriggerVibration(PresetType hapticType, float seconds, float vibrateAmountForSecond = 1)
        {
            _delayTween?.Kill();
            _delayTween =
                DOVirtual.Float(
                        0, 1,
                        seconds, _ => { }
                    )
                    .OnComplete(
                        () => _triggerTween?.Kill(true));

            _triggerTween?.Kill();
            _triggerTween = DOVirtual.Float(0, 1, 1f / vibrateAmountForSecond, _ => { })
                .OnComplete(() => PlayPreset(hapticType))
                .SetLoops(-1)
                .OnStart(() => PlayPreset(hapticType))
                .OnStepComplete(() => PlayPreset(hapticType));
        }

        public void TriggerVibration(PresetType[] hapticType, float seconds, bool isRandom = false, float vibrateAmountForSecond = 1)
        {
            _delayTween?.Kill();
            _delayTween =
                DOVirtual.Float(0, 1, seconds, _ => { })
                    .OnComplete(
                        () => _triggerTween?.Kill(true));

            var index = 0;

            Func<PresetType> getHapticType = null;
            if (isRandom)
            {
                getHapticType = () => hapticType[Random.Range(0, hapticType.Length)];
            }
            else
            {
                getHapticType = () => hapticType[index++ % hapticType.Length];
            }

            _triggerTween?.Kill();
            _triggerTween = DOVirtual.Float(0, 1, 1f / vibrateAmountForSecond, _ => { })
                .OnComplete(
                    () => PlayPreset(getHapticType()))
                .SetLoops(-1)
                .OnStart(() => PlayPreset(getHapticType()))
                .OnStepComplete(() => PlayPreset(getHapticType()));
        }

        public void ReleaseVibration()
        {
            _delayTween?.Kill(true);
            _triggerTween?.Kill(true);
        }
    }
}