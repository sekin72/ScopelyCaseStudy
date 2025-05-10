using System;
using CerberusFramework.Core.Managers.Inventory;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using UnityEngine;

namespace CerberusFramework.Core.UI.FlyTweens.Inventory
{
    public abstract class InventoryFlyTweenData : FlyTweenData
    {
        public readonly ResourceData ResourceData;
        public readonly int AdditionalViewOrder;

        protected InventoryFlyTweenData(
            PoolKeys poolKey,
            Transform parent,
            Transform target,
            UIContainer uiContainer,
            Action onComplete,
            ResourceData resourceData,
            int additionalViewOrder = 100) :
            base(poolKey, parent, target, uiContainer, onComplete)
        {
            ResourceData = resourceData;
            AdditionalViewOrder = additionalViewOrder;
        }
    }
}