namespace CerberusFramework.Core.Managers.Asset.Helpers
{
    public enum DependencyDownloadStatus
    {
        Unknown = 0,
        Queued = 1,
        Downloading = 2,
        Downloaded = 3,
        InBuild = 4,
        Timeout = 5,
        Exception = 6
    }
}