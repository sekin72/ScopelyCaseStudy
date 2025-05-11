using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using VContainer;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public class CreepBase : Enemy
    {
        private TowardsTargetMovementComponent _towardsTargetMovementComponent;
        private CreepAttackComponent _creepAttackComponent;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            _towardsTargetMovementComponent = MovementComponent as TowardsTargetMovementComponent;
            _creepAttackComponent = AttackComponent as CreepAttackComponent;

            _towardsTargetMovementComponent.SetTarget(GameSession.Base.View.transform);

            _towardsTargetMovementComponent.Initialize(GameSession, this, Data.EnemyConfig);
            LifeComponent.Initialize(GameSession, this, Data.EnemyConfig);
            _creepAttackComponent.Initialize(GameSession, this, Data.EnemyConfig);
        }
    }
}
