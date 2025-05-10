using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.LevelAssets
{
    public class SpawnPointView : View
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
