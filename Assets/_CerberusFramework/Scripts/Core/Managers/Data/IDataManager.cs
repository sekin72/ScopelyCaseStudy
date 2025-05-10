using CerberusFramework.Core.Managers.Data.Storages;

namespace CerberusFramework.Core.Managers.Data
{
    public interface IDataManager
    {
        T Load<T>() where T : class, IStorage, new();

        void Save<T>(T data) where T : class, IStorage, new();

        void ForceSave();
    }
}