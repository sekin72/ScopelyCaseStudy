using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause;
using ScopelyCaseStudy.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Injection
{
    public sealed class CFDemoSceneScope : LifetimeScope
    {
        public CFDemoSceneController CFDemoSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(CFDemoSceneController);
            RegisterUI(builder);
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<PausePopup>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        }
    }
}
