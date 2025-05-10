using CerberusFramework.Utilities;

namespace CerberusFramework.Core.Managers.Vibration
{
    public enum VibrationType
    {
        None = -1,
        Selection = 0,
        Success = 1,
        Warning = 2,
        Failure = 3,
        LightImpact = 4,
        MediumImpact = 5,
        HeavyImpact = 6,
        RigidImpact = 7,
        SoftImpact = 8,
    }
}