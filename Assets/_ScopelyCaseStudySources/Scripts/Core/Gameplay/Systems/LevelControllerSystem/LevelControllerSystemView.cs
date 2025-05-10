using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CerberusFramework.Core.MVC;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.LevelAssets;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Systems.LevelControllerSystem
{
    public class LevelControllerSystemView : View
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

        public void AttachLevel(LevelView level)
        {
            level.transform.SetParent(transform);
            level.transform.localPosition = Vector3.zero;
            level.transform.localRotation = Quaternion.identity;
            level.transform.localScale = Vector3.one;
        }
    }
}
