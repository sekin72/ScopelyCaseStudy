using CerberusFramework.Core.Managers;

namespace CerberusFramework.Core.Events
{
    public readonly struct ManagerStateChangedEvent
    {
        public readonly IManager Manager;
        public readonly ManagerState OldState;
        public readonly ManagerState NewState;

        public ManagerStateChangedEvent(IManager manager, ManagerState oldState, ManagerState newState)
        {
            Manager = manager;
            OldState = oldState;
            NewState = newState;
        }
    }
}