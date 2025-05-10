using System;
using System.Collections.Generic;

namespace ScopelyCaseStudy.Core.Gameplay.GameData
{
    [Serializable]
    public struct WaveData
    {
        public int Score;
        public float TimeInterval;
        public List<int> EnemyPoolKeys;

        public WaveData(int score, float timeInterval, List<int> enemyPoolKeys)
        {
            Score = score;
            TimeInterval = timeInterval;
            EnemyPoolKeys = enemyPoolKeys;
        }
    }
}
