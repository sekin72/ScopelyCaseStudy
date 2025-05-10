using System;
using System.Threading;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win
{
    public class WinPopupView : PopupView
    {
        [SerializeField] protected CFButton BackToMainMenuButton;

        public event Action BackToMainMenuButtonClicked;

        public override UniTask Initialize(CancellationToken cancellationToken)
        {
            base.Initialize(cancellationToken);

            BackToMainMenuButton.onClick.AddListener(() => BackToMainMenuButtonClicked?.Invoke());

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            BackToMainMenuButton.onClick.RemoveListener(() => BackToMainMenuButtonClicked?.Invoke());

            base.Dispose();
        }
    }
}