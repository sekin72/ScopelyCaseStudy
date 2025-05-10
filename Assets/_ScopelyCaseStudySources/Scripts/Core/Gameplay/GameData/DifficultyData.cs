using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.GameData
{
    [CreateAssetMenu(fileName = "DifficultyData", menuName = "ScopelyCaseStudy/GameData/DifficultyData", order = 2)]
    public class DifficultyData : ScriptableObject
    {
        public List<WaveData> WaveDatas = new List<WaveData>()
        {
            new WaveData(10, 5f, new List<int> { 200 }),
            new WaveData(25, 4.5f, new List<int> { 200 }),
            new WaveData(50, 4f, new List<int> { 200, 201 }),
            new WaveData(100, 3.5f, new List<int> { 200, 201 }),
            new WaveData(200, 2.5f, new List<int> { 200, 201 }),
            new WaveData(300, 2f, new List<int> { 200, 201, 202 }),
            new WaveData(500, 1.5f, new List<int> { 200, 201, 202 }),
            new WaveData(750, 1f, new List<int> { 200, 201, 202 }),
        };
    }
}
