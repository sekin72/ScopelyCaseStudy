using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Systems.EnemyControllerSystem;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    public class Turret : Character<TurretData, TurretView>
    {
        private readonly List<Enemy> _enemyList = new List<Enemy>();

        protected IEnemyControllerSystem EnemyControllerSystem;

        protected Enemy ClosestEnemy;

        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            EnemyControllerSystem = GameSession.GetSystem<IEnemyControllerSystem>();

            ClosestEnemy = null;

            AttackComponent.Initialize(GameSession, this, Data.TurretConfig);
        }

        protected override void Activate()
        {
            base.Activate();

            View.EnemyEnteredReach += View_EnemyEnteredReach;
            View.EnemyExitedReach += View_EnemyExitedReach;
        }

        protected override void Deactivate()
        {
            View.EnemyEnteredReach -= View_EnemyEnteredReach;
            View.EnemyExitedReach -= View_EnemyExitedReach;
        }

        protected override void Dispose()
        {
            AttackComponent.Dispose();
            base.Dispose();
        }

        public override void OnDeath()
        {
        }

        public override void LateTick()
        {
            base.LateTick();

            for (var i = 0; i < _enemyList.Count; i++)
            {
                if (!_enemyList[i].IsAlive())
                {
                    _enemyList.RemoveAt(i);
                    i--;
                }
            }

            if (_enemyList.Count <= 0)
            {
                ClosestEnemy = null;
                return;
            }

            var enemy = _enemyList.FirstOrDefault(x => x.IsAlive());

            var curDistance = Vector3.Distance(View.transform.position, enemy.View.transform.position);
            for (var i = 0; i < _enemyList.Count; i++)
            {
                if (!_enemyList[i].IsAlive())
                {
                    continue;
                }

                var distance = Vector3.Distance(View.transform.position, _enemyList[i].View.transform.position);
                if (distance < curDistance)
                {
                    enemy = _enemyList[i];
                    curDistance = distance;
                }
            }

            if (ClosestEnemy != enemy)
            {
                ClosestEnemy = enemy;
            }

            View.TurnToClosestEnemy(ClosestEnemy.View);
        }

        private void View_EnemyEnteredReach(EnemyView enemyView)
        {
            _enemyList.Add(EnemyControllerSystem.GetEnemyFromView(enemyView));
        }

        private void View_EnemyExitedReach(EnemyView enemyView)
        {
            _enemyList.Remove(EnemyControllerSystem.GetEnemyFromView(enemyView));
        }
    }
}
