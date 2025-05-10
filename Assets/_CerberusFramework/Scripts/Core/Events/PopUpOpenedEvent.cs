namespace CerberusFramework.Core.Events
{
    public readonly struct PopUpOpenedEvent
    {
        public readonly string PopUpUniqueName;

        public PopUpOpenedEvent(string popUpUniqueName)
        {
            PopUpUniqueName = popUpUniqueName;
        }
    }
}