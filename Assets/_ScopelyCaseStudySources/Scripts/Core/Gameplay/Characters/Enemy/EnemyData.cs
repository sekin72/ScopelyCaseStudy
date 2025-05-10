using System.Collections;
using System.Collections.Generic;
using CerberusFramework.Core.MVC;
using ScopelyCaseStudy.Core.Gameplay.Characters.Enemy;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    public class EnemyData : Data
    {
        public readonly EnemyConfig EnemyConfig;

        public EnemyData(EnemyConfig enemyConfig)
        {
            EnemyConfig = enemyConfig;
        }
    }
}
