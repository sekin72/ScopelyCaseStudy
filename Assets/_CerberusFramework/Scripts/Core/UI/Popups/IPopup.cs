using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.Popups
{
    public interface IPopup : IController, IGradualLifecycle
    {
        new PopupData Data { get; }
        new PopupView View { get; }

        public string UniqueName { get; }

        void TapOutside();

        void GoBack();
    }
}