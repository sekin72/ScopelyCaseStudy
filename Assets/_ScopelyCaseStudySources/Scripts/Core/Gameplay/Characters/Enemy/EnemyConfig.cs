using System;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    [Serializable]
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "ScopelyCaseStudy/Data/EnemyConfig", order = 3)]
    public class EnemyConfig : CharacterConfig
    {
        public EnemyTypes EnemyType;

        public int RewardedCoin = 5;
    }
}
