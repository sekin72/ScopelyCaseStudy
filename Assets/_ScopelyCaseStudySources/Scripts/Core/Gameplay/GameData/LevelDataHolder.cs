using System.Collections.Generic;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.GameData
{
    [CreateAssetMenu(fileName = "LevelDataHolder", menuName = "ScopelyCaseStudy/Data/LevelDataHolder", order = 3)]
    public class LevelDataHolder : ScriptableObject
    {
        public List<LevelData> LevelData;
    }
}
