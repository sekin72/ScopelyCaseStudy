using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using ScopelyCaseStudy.Core.Gameplay.Characters.Components;
using ScopelyCaseStudy.Core.Gameplay.LevelAssets;
using ScopelyCaseStudy.Core.Scenes;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public class Base : Character<BaseData, BaseView>
    {
        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            LifeComponent.Initialize(GameSession, this, Data.BaseConfig);
            //LifeComponent.SetHealthBar(((LevelSceneController)AppManager.CurrentSceneController).PlayerHealthBar);
        }

        public override void OnDeath()
        {
            GameSession.LevelFinished(false);
        }
    }
}
