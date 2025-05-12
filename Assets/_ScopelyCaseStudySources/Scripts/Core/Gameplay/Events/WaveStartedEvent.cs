using CerberusFramework.Core.Managers.Pool;

namespace ScopelyCaseStudy.Core.Gameplay.Events
{
    public readonly struct WaveStartedEvent
    {
        public readonly int Index;

        public WaveStartedEvent(int index)
        {
            Index = index;
        }
    }
}
