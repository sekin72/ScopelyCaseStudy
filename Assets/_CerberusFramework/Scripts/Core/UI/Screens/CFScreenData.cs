using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Popups.Helpers;

namespace CerberusFramework.Core.UI.Screens
{
    public abstract class CFScreenData : Data
    {
        public readonly CFScreenAnchors ScreenAnchor;

        protected CFScreenData(CFScreenAnchors screenAnchor)
        {
            ScreenAnchor = screenAnchor;
        }
    }
}