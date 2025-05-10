using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Popups;
using ScopelyCaseStudy.Core.Scenes;

namespace ScopelyCaseStudy.Core.Gameplay.UI.Popups.Fail
{
    public class FailPopupData : PopupData
    {
        public readonly LevelSceneController LevelSceneController;

        public FailPopupData(LevelSceneController levelSceneController) : base(PoolKeys.FailPopup)
        {
            LevelSceneController = levelSceneController;
        }
    }
}