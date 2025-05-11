using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public class Base : Character<BaseData, BaseView>
    {
        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            LifeComponent.Initialize(GameSession, this, Data.BaseConfig);
        }

        protected override void Dispose()
        {
            LifeComponent.Dispose();
            base.Dispose();
        }

        public override void OnDeath()
        {
            GameSession.LevelFinished(false);
        }
    }
}
