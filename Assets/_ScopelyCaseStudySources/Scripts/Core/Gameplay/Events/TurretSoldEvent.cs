namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct TurretSoldEvent
    {
        public readonly int Cost;

        public TurretSoldEvent(int cost)
        {
            Cost = cost;
        }
    }
}
