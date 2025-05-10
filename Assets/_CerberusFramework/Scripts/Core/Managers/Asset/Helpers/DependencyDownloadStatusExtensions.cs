namespace CerberusFramework.Core.Managers.Asset.Helpers
{
    public static class DependencyDownloadStatusExtensions
    {
        public static bool NeedsChecking(this DependencyDownloadStatus status)
        {
            return status == DependencyDownloadStatus.Exception ||
                status == DependencyDownloadStatus.Unknown;
        }

        public static bool NeedsDownload(this DependencyDownloadStatus status)
        {
            return status == DependencyDownloadStatus.Queued ||
                status == DependencyDownloadStatus.Downloading ||
                status == DependencyDownloadStatus.Timeout ||
                status == DependencyDownloadStatus.Exception;
        }

        public static bool IsPresent(this DependencyDownloadStatus status)
        {
            return status == DependencyDownloadStatus.Downloaded ||
                status == DependencyDownloadStatus.InBuild;
        }
    }
}