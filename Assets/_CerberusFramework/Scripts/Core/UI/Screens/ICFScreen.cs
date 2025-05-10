using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Components;

namespace CerberusFramework.Core.UI.Screens
{
    public interface ICFScreen : IController, IGradualLifecycle
    {
        public Darkinator Darkinator { get; }
        public SafeArea SafeArea { get; }
        new CFScreenView View { get; }
        new CFScreenData Data { get; }
    }
}