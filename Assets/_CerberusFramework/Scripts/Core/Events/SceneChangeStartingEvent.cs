namespace CerberusFramework.Core.Events
{
    public readonly struct SceneChangeStartingEvent
    {
        public readonly string FromSceneName;
        public readonly string ToSceneName;

        public SceneChangeStartingEvent(string fromSceneName, string toSceneName)
        {
            FromSceneName = fromSceneName;
            ToSceneName = toSceneName;
        }
    }
}