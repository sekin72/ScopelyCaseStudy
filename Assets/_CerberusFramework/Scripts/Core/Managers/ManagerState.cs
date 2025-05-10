namespace CerberusFramework.Core.Managers
{
    public enum ManagerState
    {
        WaitingStart = 0,
        WaitingDependencies,
        Initializing,
        DependenciesFailed,
        Ready,
        Failed,
        Disabled
    }
}