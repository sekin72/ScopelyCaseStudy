using CerberusFramework.Core.MVC;

namespace CerberusFramework.Core.UI.FlyTweens
{
    public interface IFlyTween : IController, IGradualLifecycle
    {
        new FlyTweenView View { get; }
        new FlyTweenData Data { get; }
    }
}