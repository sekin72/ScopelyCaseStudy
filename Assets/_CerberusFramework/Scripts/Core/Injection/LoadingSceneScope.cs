using CerberusFramework.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace CerberusFramework.Core.Injection
{
    public sealed class LoadingSceneScope : LifetimeScope
    {
        public LoadingSceneController LoadingSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(LoadingSceneController);
        }
    }
}