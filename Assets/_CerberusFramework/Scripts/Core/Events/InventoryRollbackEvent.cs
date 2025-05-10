using CerberusFramework.Core.Managers.Inventory;

namespace CerberusFramework.Core.Events
{
    public readonly struct InventoryRollbackEvent
    {
        public readonly ResourceData ResourceData;

        public InventoryRollbackEvent(ResourceData resourceData)
        {
            ResourceData = resourceData;
        }
    }
}