using CerberusFramework.Core.UI.Components;

namespace CerberusFramework.Core.Events
{
    public readonly struct UIContainerDestroyedEvent
    {
        public readonly UIContainer UiContainer;

        public UIContainerDestroyedEvent(UIContainer uiContainer)
        {
            UiContainer = uiContainer;
        }
    }
}