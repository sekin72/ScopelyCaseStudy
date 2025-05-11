using System.Collections;
using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Weapons;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    [CreateAssetMenu(fileName = "MeleeWeaponConfig", menuName = "ScopelyCaseStudy/Data/MeleeWeaponConfig", order = 3)]
    public class MeleeWeaponConfig : WeaponConfig
    {
        public override AttackComponentTypes GetAttackComponentType() => AttackComponentTypes.MeleeAttack;
    }
}
