using static Lofelt.NiceVibrations.HapticPatterns;

namespace CerberusFramework.Core.Managers.Vibration
{
    public interface IVibrationConfiguration
    {
        void Vibrate(PresetType hapticType);

        void TriggerVibration(PresetType hapticType, float seconds, float vibrateAmountForSecond = 1);

        void TriggerVibration(PresetType[] hapticType, float seconds, bool isRandom = false, float vibrateAmountForSecond = 1);

        void ReleaseVibration();
    }
}