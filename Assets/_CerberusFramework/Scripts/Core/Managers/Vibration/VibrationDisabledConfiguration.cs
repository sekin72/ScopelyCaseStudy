using Lofelt.NiceVibrations;

namespace CerberusFramework.Core.Managers.Vibration
{
    public class VibrationDisabledConfiguration : IVibrationConfiguration
    {
        public void ReleaseVibration()
        {
        }

        public void TriggerVibration(HapticPatterns.PresetType hapticType, float seconds, float vibrateAmountForSecond = 1)
        {
        }

        public void TriggerVibration(HapticPatterns.PresetType[] hapticType, float seconds, bool isRandom = false, float vibrateAmountForSecond = 1)
        {
        }

        public void Vibrate(HapticPatterns.PresetType hapticType)
        {
        }
    }
}