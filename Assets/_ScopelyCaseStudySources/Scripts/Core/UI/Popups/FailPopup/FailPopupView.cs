using System;
using System.Threading;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail
{
    public class FailPopupView : PopupView
    {
        [SerializeField] protected CFButton RestartButton;

        public event Action RestartButtonClicked;

        public override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            RestartButton.onClick.AddListener(() => RestartButtonClicked?.Invoke());
        }

        public override void Dispose()
        {
            RestartButton.onClick.RemoveAllListeners();

            base.Dispose();
        }
    }
}