using System;
using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Characters;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.GameData
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScopelyCaseStudy/Data/LevelData", order = 3)]
    public class LevelData : ScriptableObject
    {
        public List<WaveConfig> WaveData;
        public BaseConfig BaseConfig;
    }

    [Serializable]
    public struct EnemyConfigCountPair
    {
        public EnemyConfig EnemyConfigs;
        public int Count;
    }

    [Serializable]
    public struct WaveConfig
    {
        public List<EnemyConfigCountPair> EnemyLists;

        public float SpawnDelay;
        public float SpawnInterval;
    }
}
