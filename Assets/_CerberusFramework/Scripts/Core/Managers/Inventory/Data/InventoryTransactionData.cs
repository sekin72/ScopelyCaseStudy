using System;

namespace CerberusFramework.Core.Managers.Inventory
{
    public class InventoryTransactionData : ICloneable
    {
        public string Reason;
        public ResourceData ResourceData;

        public InventoryTransactionData()
        {
        }

        public InventoryTransactionData(InventoryTransactionData source)
        {
            Reason = source.Reason;
            ResourceData = new ResourceData(source.ResourceData);
        }

        public InventoryTransactionData(ResourceData resourceData, string reason)
        {
            ResourceData = resourceData;
            Reason = reason;
        }

        public object Clone()
        {
            return new InventoryTransactionData(this);
        }
    }
}