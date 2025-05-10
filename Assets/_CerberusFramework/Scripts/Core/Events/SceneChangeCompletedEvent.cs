namespace CerberusFramework.Core.Events
{
    public readonly struct SceneChangeCompletedEvent
    {
        public readonly string FromSceneName;
        public readonly string ToSceneName;

        public SceneChangeCompletedEvent(string fromSceneName, string toSceneName)
        {
            FromSceneName = fromSceneName;
            ToSceneName = toSceneName;
        }
    }
}