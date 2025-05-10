using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Enemy
{
    public class EnemyConfig : ScriptableObject
    {
        public PoolKeys PoolKey;
        public EnemyTypes EnemyType;

        public int RewardedCoin = 5;
    }
}
