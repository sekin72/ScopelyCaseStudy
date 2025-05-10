using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    public class EnemyView : View
    {
        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public override void Activate()
        {
        }

        public override void Deactivate()
        {
        }

        public override void Dispose()
        {
        }
    }
}
