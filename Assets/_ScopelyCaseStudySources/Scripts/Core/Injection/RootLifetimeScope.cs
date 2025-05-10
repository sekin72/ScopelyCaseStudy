using CerberusFramework.Config;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Injection;
using CerberusFramework.Core.Managers.Asset;
using CerberusFramework.Core.Managers.Data;
using CerberusFramework.Core.Managers.DeviceConfiguration;
using CerberusFramework.Core.Managers.Inventory;
using CerberusFramework.Core.Managers.Loading;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.Managers.Sound;
using CerberusFramework.Core.Managers.UI;
using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Core.UI.FlyTweens.Coin;
using CerberusFramework.Core.UI.FlyTweens.Star;
using CerberusFramework.Core.UI.Popups.CheckConnection;
using CerberusFramework.Core.UI.Popups.Loading;
using CerberusFramework.Core.UI.Popups.RemoteAssetDownload;
using CerberusFramework.Core.UI.Popups.Settings;
using CerberusFramework.Utilities.MonoBehaviourUtilities;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Pause;
using ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ScopelyCaseStudy.Core.Injection
{
    public class RootLifetimeScope : LifetimeScope
    {
        public SceneReferencesHolder SceneReferencesHolder;
        public SOAssetReferenceHolder SOAssetReferenceHolder;

        protected override void Configure(IContainerBuilder builder)
        {
            TimeHelper.Initialize();

            var messagePipeOptions = RegisterMessagePipe(builder);
            RegisterManagers(builder, SceneReferencesHolder, SOAssetReferenceHolder);
            RegisterUI(builder);

            ProjectConfiguration(builder, messagePipeOptions);
        }

        protected virtual void ProjectConfiguration(IContainerBuilder builder, MessagePipeOptions messagePipeOptions)
        {
            Application.targetFrameRate = 60;
        }

        private static MessagePipeOptions RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe(
                pipeOptions =>
                {
                    if (Application.isEditor)
                    {
                        pipeOptions.EnableCaptureStackTrace = true;
                    }
                }
            );

            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            builder.RegisterMessageBroker<ApplicationPauseEvent>(options);
            builder.RegisterMessageBroker<ApplicationQuitEvent>(options);
            builder.RegisterMessageBroker<InventoryChangedEvent>(options);
            builder.RegisterMessageBroker<InventoryRollbackEvent>(options);
            builder.RegisterMessageBroker<LevelStartedEvent>(options);
            builder.RegisterMessageBroker<LoadingCompletedEvent>(options);
            builder.RegisterMessageBroker<ManagerStateChangedEvent>(options);
            builder.RegisterMessageBroker<PopUpOpenedEvent>(options);
            builder.RegisterMessageBroker<PopupClosedEvent>(options);
            builder.RegisterMessageBroker<RemoteAssetDependencyDownloadStatusChangedEvent>(options);
            builder.RegisterMessageBroker<SceneChangeCompletedEvent>(options);
            builder.RegisterMessageBroker<SceneChangeStartingEvent>(options);
            builder.RegisterMessageBroker<UIContainerCreatedEvent>(options);
            builder.RegisterMessageBroker<UIContainerDestroyedEvent>(options);
            builder.RegisterMessageBroker<ScreenChangedEvent>(options);

            builder.RegisterMessageBroker<AttachParticleEvent>(options);
            builder.RegisterMessageBroker<DetachParticleEvent>(options);
            builder.RegisterMessageBroker<InputTakenEvent>(options);
            return options;
        }

        private static void RegisterManagers(
            IContainerBuilder builder,
            SceneReferencesHolder sceneReferencesHolder,
            SOAssetReferenceHolder soAssetReferenceHolder)
        {
            builder.Register<DeviceConfigurationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<LoadingManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf().WithParameter(sceneReferencesHolder);
            builder.Register<DataManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AssetManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf().WithParameter(soAssetReferenceHolder);
            builder.Register<PoolManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SoundManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<VibrationManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PopupManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PoolWarmUpManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AddressableManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<RemoteAssetManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<TextTweenManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<InventoryManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<FlyTweenManager>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<LoadingPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<WinPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<FailPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<SettingsPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<CheckConnectionPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<RemoteAssetDownloadPopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<PausePopup>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<StarFlyTween>(Lifetime.Transient).AsImplementedInterfaces().AsSelf();
            builder.Register<CoinFlyTween>(Lifetime.Transient).AsImplementedInterfaces().AsSelf();
        }
    }
}