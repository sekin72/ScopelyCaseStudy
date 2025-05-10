namespace CerberusFramework.Core.Managers.Asset.Helpers
{
    public interface ILoadProgress
    {
        string LoadName { get; }
        float LoadProgress();
        string GetDescription();
    }
}