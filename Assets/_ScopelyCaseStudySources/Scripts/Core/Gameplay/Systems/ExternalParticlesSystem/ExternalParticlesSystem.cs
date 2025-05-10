using System;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay;
using ScopelyCaseStudy.Core.Gameplay.Events;
using ScopelyCaseStudy.Core.Gameplay.Systems;
using UnityEngine;
using VContainer;

namespace CFGameClient.Core.Gameplay.Systems.ExternalParticles
{
    [CreateAssetMenu(fileName = "ExternalParticlesSystem", menuName = "CerberusFramework/Systems/ExternalParticlesSystem", order = 3)]
    public class ExternalParticlesSystem : GameSystem, IExternalParticlesSystem
    {
        private IDisposable _messageSubscription;
        private ExternalParticlesSystemView _view;
        public override Type RegisterType => typeof(IExternalParticlesSystem);

        private PoolManager _poolManager;

        [Inject]
        private void Inject(PoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public override async UniTask Initialize(GameSession gameSession, CancellationToken cancellationToken)
        {
            await base.Initialize(gameSession, cancellationToken);

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<AttachParticleEvent>().Subscribe(OnAttachParticleEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _view = _poolManager.GetGameObject(PoolKeys.ExternalParticlesSystemView).GetComponent<ExternalParticlesSystemView>();
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
            _messageSubscription?.Dispose();
            _poolManager.SafeReleaseObject(PoolKeys.ExternalParticlesSystemView, _view.gameObject);
        }

        private void OnAttachParticleEvent(AttachParticleEvent evt)
        {
            _view.AttachGameObject(evt.PoolKey, evt.ParticleGameObject);
        }
    }
}