using System.Collections.Generic;
using CerberusFramework.Core.Managers.Pool;
using UnityEngine;

namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class GameSessionSaveStorage : IStorage
    {
        public bool GameplayFinished = false;
        public int CurrentLevel = 0;
        public int LevelRandomSeed;

        public int HighScore;

        public int WaveIndex = 0;
        public int Gold = 10;
        public int CurrentScore;
        public List<Vector3> PlacedTurretPositions = new List<Vector3>();
        public List<PoolKeys> PlacedTurretPoolKeys = new List<PoolKeys>();
    }
}