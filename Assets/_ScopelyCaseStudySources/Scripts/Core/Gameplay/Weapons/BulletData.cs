using System.Collections.Generic;
using ScopelyCaseStudy.Core.Gameplay.Effects;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public readonly struct BulletData
    {
        public readonly float Damage;
        public readonly float Range;
        public readonly float BulletLifetime;
        public readonly float BulletTravelSpeed;
        public readonly List<Effect> AdditionalEffects;
        public readonly Color Color;

        public BulletData(
            float damage,
            float range,
            float bulletLifetime,
            float bulletTravelSpeed,
            List<Effect> additionalEffects,
            Color color)
        {
            Damage = damage;
            Range = range;
            BulletLifetime = bulletLifetime;
            BulletTravelSpeed = bulletTravelSpeed;
            AdditionalEffects = additionalEffects;
            Color = color;
        }
    }
}
