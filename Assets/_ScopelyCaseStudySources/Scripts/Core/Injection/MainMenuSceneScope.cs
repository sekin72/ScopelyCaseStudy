using CerberusFramework.Core.UI.Screens.Main;
using ScopelyCaseStudy.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Injection
{
    public sealed class MainMenuSceneScope : LifetimeScope
    {
        public MainSceneController MainSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(MainSceneController);
            builder.Register<MainScreen>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }
    }
}