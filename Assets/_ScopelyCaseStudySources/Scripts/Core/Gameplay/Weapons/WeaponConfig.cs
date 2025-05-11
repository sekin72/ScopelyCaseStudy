using ScopelyCaseStudy.Core.Gameplay.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public abstract class WeaponConfig : ScriptableObject
    {
        public abstract AttackComponentTypes GetAttackComponentType();

        public int Cooldown;
        public int Damage;
        public int Range;

        public List<Effect> AdditionalEffects;
    }
}
