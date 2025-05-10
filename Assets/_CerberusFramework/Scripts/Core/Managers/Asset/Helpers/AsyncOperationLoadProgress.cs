using UnityEngine.ResourceManagement.AsyncOperations;

namespace CerberusFramework.Core.Managers.Asset.Helpers
{
    public class AsyncOperationLoadProgress : ILoadProgress
    {
        private string _description = "";
        private float _progress;

        public AsyncOperationLoadProgress(string name)
        {
            LoadName = name;
        }

        public bool Succeeded => Status == AsyncOperationStatus.Succeeded;
        public bool IsDone => Status != AsyncOperationStatus.None;
        public AsyncOperationStatus Status { get; private set; } = AsyncOperationStatus.None;

        public AsyncOperationHandle Handle { get; private set; }
        public bool HandleSet { get; private set; }
        public string LoadName { get; }

        public float LoadProgress()
        {
            if (!HandleSet)
            {
                return 0;
            }

            if (IsDone)
            {
                return _progress;
            }

            var status = Handle.GetDownloadStatus();
            _progress = status.TotalBytes == 0 ? 0 : (float)status.DownloadedBytes / status.TotalBytes;
            return _progress;
        }

        public string GetDescription()
        {
            if (!HandleSet)
            {
                return _description;
            }

            if (IsDone)
            {
                return _description;
            }

            var status = Handle.GetDownloadStatus();

            var downloaded = ByteToMBString(status.DownloadedBytes);
            var total = ByteToMBString(status.TotalBytes);

            _description = $"{downloaded}/{total}";
            return _description;
        }

        public void SetHandle(AsyncOperationHandle value)
        {
            Handle = value;
            Handle.Completed += OnHandleCompleted;
            HandleSet = true;
        }

        private void OnHandleCompleted(AsyncOperationHandle obj)
        {
            var status = Handle.GetDownloadStatus();
            var downloaded = ByteToMBString(status.DownloadedBytes);
            var total = ByteToMBString(status.TotalBytes);

            _description = $"{downloaded}/{total}";
            _progress = status.TotalBytes == 0 ? 0 : (float)status.DownloadedBytes / status.TotalBytes;

            Status = obj.Status;
            obj.Completed -= OnHandleCompleted;
        }

        public static string ByteToString(long b)
        {
            if(b >= 1000000)
            {
                return (b / 1000000) + "MB";
            }

            if (b >= 1000)
            {
                return (b / 1000) + "KB";
            }

            return b + "B";
        }

        public static string ByteToMBString(long b)
        {
            return $"{(int)(b / 1000000)} MB";
        }
    }
}