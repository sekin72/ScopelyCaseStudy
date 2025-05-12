using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;

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

            _creepAttackComponent.Attacked += OnAttacked;

            _towardsTargetMovementComponent.SetTarget(GameSession.Base.View.transform);
        }

        protected override void Deactivate()
        {
            _creepAttackComponent.Attacked -= OnAttacked;
            base.Deactivate();
        }

        private void OnAttacked()
        {
            //TakeDamage(Data.EnemyConfig.Health);
        }
    }
}
