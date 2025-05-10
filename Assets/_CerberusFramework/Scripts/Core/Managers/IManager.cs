namespace CerberusFramework.Core.Managers
{
    public interface IManager
    {
        ManagerState ManagerState { get; }
        bool IsCore { get; }
    }
}