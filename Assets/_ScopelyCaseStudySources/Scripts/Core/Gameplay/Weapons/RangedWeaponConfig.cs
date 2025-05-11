using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "RangedWeaponConfig", menuName = "ScopelyCaseStudy/Data/RangedWeaponConfig", order = 3)]
    public class RangedWeaponConfig : WeaponConfig
    {
        public override AttackComponentTypes GetAttackComponentType() => AttackComponentTypes.RangedAttack;

        public float BulletLifetime;
        public float BulletTravelSpeed;
        public Color BulletColor;
    }
}
