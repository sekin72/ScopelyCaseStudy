namespace CerberusFramework.Core.Managers.Data.Storages
{
    public class GameSessionSaveStorage : IStorage
    {
        public bool GameplayFinished = false;
        public int CurrentLevel = 0;
        public int LevelRandomSeed = 0;
    }
}