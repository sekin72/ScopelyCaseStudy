namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    public class TurretData : CharacterData
    {
        public TurretConfig TurretConfig { get; private set; }

        public TurretData(TurretConfig turretConfig): base(turretConfig.MoveSpeed)
        {
            TurretConfig = turretConfig;
        }
    }
}
