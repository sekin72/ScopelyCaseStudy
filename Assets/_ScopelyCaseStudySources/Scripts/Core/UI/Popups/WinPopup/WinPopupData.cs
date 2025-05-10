using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Popups;
using ScopelyCaseStudy.Core.Scenes;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Win
{
    public class WinPopupData : PopupData
    {
        public readonly LevelSceneController LevelSceneController;

        public WinPopupData(LevelSceneController levelSceneController)
            : base(PoolKeys.WinPopup)
        {
            LevelSceneController = levelSceneController;
        }
    }
}