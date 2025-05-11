namespace ScopelyCaseStudy.Core.Gameplay.Characters
{
    public class EnemyData : CharacterData
    {
        public readonly EnemyConfig EnemyConfig;

        public EnemyData(EnemyConfig enemyConfig) : base(enemyConfig.MoveSpeed)
        {
            EnemyConfig = enemyConfig;
        }
    }
}
