using System;
using System.Threading;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    public abstract class Enemy : Controller<EnemyData, EnemyView>
    {
        public event Action<Enemy> EnemyDiedEvent;

        public PoolKeys PoolKey => Data.EnemyConfig.PoolKey;

        protected override UniTask Initialize(CancellationToken cancellationToken)
        {
            return View.Initialize(cancellationToken);
        }

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
        }

        protected override void Dispose()
        {
        }

        public void OnDeath()
        {
            View.gameObject.SetActive(false);
            EnemyDiedEvent?.Invoke(this);
        }
    }
}
