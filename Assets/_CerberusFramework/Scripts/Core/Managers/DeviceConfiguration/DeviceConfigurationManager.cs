using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CerberusFramework.Core.Managers.DeviceConfiguration
{
    public class DeviceConfigurationManager : Manager
    {
        public override bool IsCore => true;

        public bool IsPC { get; private set; }
        public bool IsMobile { get; private set; }
        public bool IsIOS { get; private set; }
        public bool IsAndroid { get; private set; }
        public bool IsEditor { get; private set; }

        protected override async UniTask Initialize(CancellationToken disposeToken)
        {
            IsPC = false;
            IsMobile = false;
            IsIOS = false;
            IsAndroid = false;
            IsEditor = false;

            if (Application.isMobilePlatform)
            {
                IsMobile = true;
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                IsAndroid = true;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                IsIOS = true;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                IsEditor = true;
            }

            if (Application.platform == RuntimePlatform.WindowsPlayer && !IsEditor)
            {
                IsPC = true;
            }

            await DeviceConfiguration(disposeToken);

            SetReady();
        }

        public virtual UniTask DeviceConfiguration(CancellationToken disposeToken)
        {
            return UniTask.CompletedTask;
        }
    }
}