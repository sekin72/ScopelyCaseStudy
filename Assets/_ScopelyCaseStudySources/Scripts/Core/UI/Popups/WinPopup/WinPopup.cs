using System.Threading;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win
{
    public class WinPopup : Popup<WinPopupData, WinPopupView>
    {
        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.BackToMainMenuButtonClicked += OnBackToMainClicked;
            View.CloseButtonClicked += OnBackToMainClicked;
        }

        protected override void Dispose()
        {
            View.BackToMainMenuButtonClicked -= OnBackToMainClicked;
            View.CloseButtonClicked -= OnBackToMainClicked;

            base.Dispose();
        }

        private void OnBackToMainClicked()
        {
            Data.LevelSceneController.ReturnToMainScene();
        }
    }
}