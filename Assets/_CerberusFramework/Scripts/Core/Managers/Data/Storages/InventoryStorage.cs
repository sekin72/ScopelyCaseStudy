using System;
using System.Collections.Generic;
using CerberusFramework.Core.Managers.Inventory;

namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class InventoryStorage : IStorage
    {
        public Dictionary<int, int> InventoryItems = new Dictionary<int, int>();

        public Dictionary<long, InventoryTransactionData> TransactionItems = new Dictionary<long, InventoryTransactionData>();

        public void CopyFrom(IStorage source)
        {
            var inventoryStorage = source as InventoryStorage ?? throw new InvalidOperationException($"Source is not {nameof(InventoryStorage)}");

            InventoryItems.Clear();
            foreach (var pair in inventoryStorage.InventoryItems)
            {
                InventoryItems.Add(pair.Key, pair.Value);
            }

            TransactionItems.Clear();
            foreach (var pair in inventoryStorage.TransactionItems)
            {
                TransactionItems.Add(pair.Key, pair.Value);
            }
        }

        public object Clone()
        {
            var clone = new InventoryStorage();
            clone.CopyFrom(this);
            return clone;
        }
    }
}