using CerberusFramework.Core.UI.Components;

namespace CerberusFramework.Core.Events
{
    public readonly struct UIContainerCreatedEvent
    {
        public readonly UIContainer UiContainer;

        public UIContainerCreatedEvent(UIContainer uiContainer)
        {
            UiContainer = uiContainer;
        }
    }
}