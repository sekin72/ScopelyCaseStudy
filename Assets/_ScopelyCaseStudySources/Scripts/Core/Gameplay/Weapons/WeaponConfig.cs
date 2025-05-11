using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public abstract class WeaponConfig : ScriptableObject
    {
        public abstract AttackComponentTypes GetAttackComponentType();

        public int AttackSpeed;
        public int Damage;
        public int Range;
    }
}
