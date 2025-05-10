namespace CerberusFramework.Core.Managers.Data.Storages
{
    public interface IStorageContainer
    {
        public string DeviceId { get; set; }
        public long Timestamp { get; set; }

        T Get<T>() where T : class, IStorage, new();

        void Set<T>(T data) where T : class, IStorage, new();
    }
}