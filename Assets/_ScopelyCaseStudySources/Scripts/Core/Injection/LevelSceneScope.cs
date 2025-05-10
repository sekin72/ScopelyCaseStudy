using ScopelyCaseStudy.Core.Gameplay;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause;
using ScopelyCaseStudy.Core.Scenes;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Injection
{
    public class LevelSceneScope : LifetimeScope
    {
        public LevelSceneController LevelSceneController;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(LevelSceneController);
            builder.Register<GameSession>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
            RegisterUI(builder);
            base.Configure(builder);
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<PausePopup>(Lifetime.Scoped).AsImplementedInterfaces().AsSelf();
        }
    }
}