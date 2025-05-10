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
        }

        protected override void Dispose()
        {
            View.RestartButtonClicked -= OnRestartClicked;

            base.Dispose();
        }

        private void OnRestartClicked()
        {
            Data.LevelSceneController.RestartLevel();
        }
    }
}