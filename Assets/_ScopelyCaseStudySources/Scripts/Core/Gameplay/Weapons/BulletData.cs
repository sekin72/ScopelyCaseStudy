namespace ScopelyCaseStudy.Core.Gameplay.Weapons
{
    public readonly struct BulletData
    {
        public readonly float Damage;
        public readonly float Range;
        public readonly float BulletLifetime;
        public readonly float BulletTravelSpeed;

        public BulletData(float damage, float range, float bulletLifetime, float bulletTravelSpeed)
        {
            Damage = damage;
            Range = range;
            BulletLifetime = bulletLifetime;
            BulletTravelSpeed = bulletTravelSpeed;
        }
    }
}
