using CerberusFramework.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace CerberusFramework.Core.Injection
{
    public sealed class PreloaderSceneScope : LifetimeScope
    {
        public PreloaderSceneController PreloaderSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(PreloaderSceneController);

            builder.RegisterEntryPoint<PreloaderScene>();
        }
    }
}