using CerberusFramework.Core.Managers.Inventory;

namespace CerberusFramework.Core.Events
{
    public readonly struct InventoryChangedEvent
    {
        public readonly ResourceKeys Type;

        public InventoryChangedEvent(ResourceKeys type)
        {
            Type = type;
        }
    }
}