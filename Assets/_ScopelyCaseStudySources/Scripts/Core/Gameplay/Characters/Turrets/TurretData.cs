using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    public class TurretData : CharacterData
    {
        public TurretConfig TurretConfig { get; private set; }
        public Vector3 Position { get; private set; }

        public TurretData(TurretConfig turretConfig, Vector3 position) : base(turretConfig.MoveSpeed)
        {
            TurretConfig = turretConfig;
            Position = position;
        }
    }
}
