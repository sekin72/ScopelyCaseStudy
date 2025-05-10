using CerberusFramework.Core.Managers.Asset.Helpers;

namespace CerberusFramework.Core.Events
{
    public readonly struct RemoteAssetDependencyDownloadStatusChangedEvent
    {
        public DependencyInfo DependencyInfo { get; }

        public RemoteAssetDependencyDownloadStatusChangedEvent(DependencyInfo dependencyInfo)
        {
            DependencyInfo = dependencyInfo;
        }
    }
}