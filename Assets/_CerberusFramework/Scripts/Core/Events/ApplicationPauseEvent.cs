namespace CerberusFramework.Core.Events
{
    public readonly struct ApplicationPauseEvent
    {
        public readonly bool IsPaused;
        public readonly float TimePassedInSeconds;

        public ApplicationPauseEvent(bool isPaused, float timePassedInSeconds)
        {
            IsPaused = isPaused;
            TimePassedInSeconds = timePassedInSeconds;
        }
    }
}