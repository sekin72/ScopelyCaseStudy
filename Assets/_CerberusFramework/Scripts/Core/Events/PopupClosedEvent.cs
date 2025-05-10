using System;

namespace CerberusFramework.Core.Events
{
    public readonly struct PopupClosedEvent
    {
        public readonly string PopupUniqueName;
        public readonly Type PopupType;

        public PopupClosedEvent(Type popupType, string popupUniqueName)
        {
            PopupUniqueName = popupUniqueName;
            PopupType = popupType;
        }
    }
}