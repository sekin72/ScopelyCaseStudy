namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class StorageProperty
    {
        public delegate IStorage GetIStorage();

        public delegate void SetIStorage(IStorage data);

        public GetIStorage Get;
        public SetIStorage Set;
    }
}