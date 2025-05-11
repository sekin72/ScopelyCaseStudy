using CerberusFramework.Core.Managers.Pool;
using ScopelyCaseStudy.Core.Gameplay.Weapons;
using UnityEngine;

namespace ScopelyCaseStudy
{
    public abstract class CharacterConfig : ScriptableObject
    {
        public PoolKeys PoolKey;

        public int Health;
        public WeaponConfig WeaponConfig;
        public float MoveSpeed = 1;
    }
}
