using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Characters.Turrets
{
    [CreateAssetMenu(fileName = "TurretConfig", menuName = "ScopelyCaseStudy/Data/TurretConfig", order = 3)]
    public class TurretConfig : CharacterConfig
    {
        public int Cost = 1;
        public float CostMultiplier = 1.2f;
    }
}
