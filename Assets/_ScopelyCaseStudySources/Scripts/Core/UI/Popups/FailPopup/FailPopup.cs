using System.Threading;
using CerberusFramework.Core.UI.Popups;
using Cysharp.Threading.Tasks;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail
{
    public class FailPopup : Popup<FailPopupData, FailPopupView>
    {
        protected override async UniTask Initialize(CancellationToken cancellationToken)
        {
            await base.Initialize(cancellationToken);

            View.RestartButtonClicked += OnRestartClicked;
            View.CloseButtonClicked += OnBackToMainClicked;
        }

        protected override void Dispose()
        {
            View.RestartButtonClicked -= OnRestartClicked;
            View.CloseButtonClicked -= OnBackToMainClicked;

            base.Dispose();
        }

        private void OnRestartClicked()
        {
            Data.LevelSceneController.RestartLevel();
        }

        private void OnBackToMainClicked()
        {
            Data.LevelSceneController.ReturnToMainScene();
        }
    }
}