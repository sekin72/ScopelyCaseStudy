using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class SaveStorage : IStorageContainer
    {
        [JsonIgnore][NonSerialized] protected readonly Dictionary<Type, StorageProperty> Storages = new Dictionary<Type, StorageProperty>(32);

        public SaveStorage()
        {
            RegisterStorages();
        }

        public SettingsStorage Settings { get; set; }
        public InventoryStorage Inventory { get; set; }
        public GameSessionSaveStorage GameSessionSaveStorage { get; set; }
        public string DeviceId { get; set; }
        public long Timestamp { get; set; }

        public T Get<T>() where T : class, IStorage, new()
        {
            return Storages.TryGetValue(typeof(T), out var storageProperty) ? (T)storageProperty.Get() : default;
        }

        public void Set<T>(T data) where T : class, IStorage, new()
        {
            if (Storages.TryGetValue(typeof(T), out var storageProperty))
            {
                storageProperty.Set(data);
            }
        }

        protected virtual void RegisterStorages()
        {
            Storages.Add(
                typeof(SettingsStorage), new StorageProperty
                {
                    Get = () => Settings,
                    Set = data => Settings = (SettingsStorage)data
                }
            );
            Storages.Add(
                typeof(InventoryStorage), new StorageProperty
                {
                    Get = () => Inventory,
                    Set = data => Inventory = (InventoryStorage)data
                }
            );
            Storages.Add(
                typeof(GameSessionSaveStorage), new StorageProperty
                {
                    Get = () => GameSessionSaveStorage,
                    Set = data => GameSessionSaveStorage = (GameSessionSaveStorage)data
                }
            );
        }
    }
}