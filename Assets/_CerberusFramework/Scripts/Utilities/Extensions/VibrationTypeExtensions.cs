using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CerberusFramework.Core.Managers.Vibration;
using static Lofelt.NiceVibrations.HapticPatterns;

namespace CerberusFramework.Utilities.Extensions
{
    public static class VibrationTypeExtensions
    {
        public static Dictionary<VibrationType, PresetType> VibrationTypeDictionary
            => new Dictionary<VibrationType, PresetType>()
            {
                {
                    VibrationType.None, PresetType.None
                },
                {
                    VibrationType.Selection, PresetType.Selection
                },
                {
                    VibrationType.Success, PresetType.Success
                },
                {
                    VibrationType.Warning, PresetType.Warning
                },
                {
                    VibrationType.Failure, PresetType.Failure
                },
                {
                    VibrationType.LightImpact, PresetType.LightImpact
                },
                {
                    VibrationType.MediumImpact, PresetType.MediumImpact
                },
                {
                    VibrationType.HeavyImpact, PresetType.HeavyImpact
                },
                {
                    VibrationType.RigidImpact, PresetType.RigidImpact
                },
                {
                    VibrationType.SoftImpact, PresetType.SoftImpact
                }
            };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PresetType GetPresetType(this VibrationType moveType)
        {
            return VibrationTypeDictionary[moveType];
        }
    }
}