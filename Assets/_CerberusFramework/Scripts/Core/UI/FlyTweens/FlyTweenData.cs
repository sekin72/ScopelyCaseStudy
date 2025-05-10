using System;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using UnityEngine;

namespace CerberusFramework.Core.UI.FlyTweens
{
    public abstract class FlyTweenData : MVC.Data
    {
        public readonly PoolKeys PoolKey;
        public readonly Transform Parent;
        public readonly Transform Target;
        public readonly UIContainer UIContainer;
        public readonly Action OnComplete;

        protected FlyTweenData(
            PoolKeys poolKey,
            Transform parent,
            Transform target,
            UIContainer uiContainer,
            Action onComplete = null)
        {
            PoolKey = poolKey;
            Parent = parent;
            Target = target;
            UIContainer = uiContainer;
            OnComplete = onComplete;
        }
    }
}