using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using Cysharp.Threading.Tasks;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public abstract class Enemy : Character<EnemyData, EnemyView>
    {
        private IPublisher<EnemyKilledEvent> _enemyKilledEventPublisher;

        public PoolKeys PoolKey => Data.EnemyConfig.PoolKey;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            _enemyKilledEventPublisher = GlobalMessagePipe.GetPublisher<EnemyKilledEvent>();

            AttackComponent.Initialize(GameSession, this, Data.EnemyConfig);
            LifeComponent.Initialize(GameSession, this, Data.EnemyConfig);
            MovementComponent.Initialize(GameSession, this, Data.EnemyConfig);
        }

        protected override void Dispose()
        {
            AttackComponent.Dispose();
            MovementComponent.Dispose();
            LifeComponent.Dispose();
            base.Dispose();
        }

        public override void OnDeath()
        {
            View.gameObject.SetActive(false);
            _enemyKilledEventPublisher.Publish(new EnemyKilledEvent(this, 1, Data.EnemyConfig.RewardedCoin));
        }
    }
}
