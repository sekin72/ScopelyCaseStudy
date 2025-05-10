using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using CerberusFramework.Core.MVC;
using CerberusFramework.Core.UI.Components;
using UnityEngine;

namespace CerberusFramework.Core.UI.Popups
{
    public abstract class PopupView : View, IGradualLifecycle
    {
        public event Action CloseButtonClicked;

        public Transform Root;
        [SerializeField] private CFButton _closeButton;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            SetRootTransform();
            _closeButton.onClick.AddListener(OnCloseButtonClicked);

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// This method is for FORCE ACTIVATION. Use carefully.
        /// </summary>
        public override void Activate()
        {
        }

        /// <summary>
        /// This method is for FORCE DEACTIVATION. Use carefully.
        /// </summary>
        public override void Deactivate()
        {
            _closeButton.onClick.RemoveAllListeners();
        }

        public UniTask ActivateGradual(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        public UniTask DeactivateGradual(CancellationToken cancellationToken)
        {
            Deactivate();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
        }

        private void SetRootTransform()
        {
            if (Root == null)
            {
                Root = transform;
            }
        }

        public void SetVisible(bool isVisible)
        {
            gameObject?.SetActive(isVisible);
        }

        public void SetCloseButtonVisible(bool isVisible)
        {
            _closeButton.gameObject.SetActive(isVisible);
        }

        private void OnCloseButtonClicked()
        {
            CloseButtonClicked?.Invoke();
        }
    }
}