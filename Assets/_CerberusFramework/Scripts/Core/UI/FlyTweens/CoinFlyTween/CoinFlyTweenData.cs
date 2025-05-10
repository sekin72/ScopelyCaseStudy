using System;
using CerberusFramework.Core.Managers.Inventory;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.FlyTweens.Inventory;
using UnityEngine;

namespace CerberusFramework.Core.UI.FlyTweens.Coin
{
    public class CoinFlyTweenData : InventoryFlyTweenData
    {
        public CoinFlyTweenData(
            ResourceData resourceData,
            Transform parent,
            Transform target,
            UIContainer uiContainer,
            Action onComplete) :
            base(PoolKeys.CoinFlyTween, parent, target, uiContainer, onComplete, resourceData)
        {
        }
    }
}