using CerberusFramework.Core.UI.Popups.Helpers;

namespace CerberusFramework.Core.Events
{
    public readonly struct ScreenChangedEvent
    {
        public readonly CFScreenAnchors ScreenAnchor;

        public ScreenChangedEvent(CFScreenAnchors screenAnchor)
        {
            ScreenAnchor = screenAnchor;
        }
    }
}