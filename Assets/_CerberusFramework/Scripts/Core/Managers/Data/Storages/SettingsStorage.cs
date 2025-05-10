namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class SettingsStorage : IStorage
    {
        public bool IsMusicActive = true;
        public bool IsSoundActive = true;
        public bool IsVibrationActive = true;
        public float MasterVolumeValue = 1f;
    }
}