using System;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public abstract class Enemy : Character<EnemyData, EnemyView>
    {
        public event Action<Enemy> EnemyDiedEvent;

        public PoolKeys PoolKey => Data.EnemyConfig.PoolKey;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);
        }

        public override void OnDeath()
        {
            View.gameObject.SetActive(false);
            EnemyDiedEvent?.Invoke(this);
        }
    }
}
