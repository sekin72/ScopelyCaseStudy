using System;
using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    [Serializable]
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScopelyCaseStudy/Data/EnemyConfig", order = 3)]
    public class EnemyConfig : ScriptableObject
    {
        public PoolKeys PoolKey;
        public EnemyTypes EnemyType;

        public int RewardedCoin = 5;
    }
}
