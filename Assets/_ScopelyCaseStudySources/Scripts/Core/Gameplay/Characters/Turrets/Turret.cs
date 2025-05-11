using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    public class Turret : Character<TurretData, TurretView>
    {
        private TurretAttackComponent _turretAttackComponent;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);
            _turretAttackComponent = AttackComponent as TurretAttackComponent;
        }

        public override void OnDeath()
        {
        }
    }
}
