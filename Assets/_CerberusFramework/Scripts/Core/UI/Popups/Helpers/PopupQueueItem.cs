using System;
using CerberusFramework.Core.Managers.UI;

namespace CerberusFramework.Core.UI.Popups.Helpers
{
    public class PopupQueueItem
    {
        public readonly bool ShouldSurviveOnSceneChange;

        public PopupQueueItem(
            PopupData data,
            bool shouldSurviveOnSceneChange,
            Action popupOpenerAction,
            QueueOpenTimeConditionEvaluate queueOpenTimeConditionEvaluate,
            CFScreenAnchors screenAnchor,
            QueuePosition queuePosition)
        {
            ShouldSurviveOnSceneChange = shouldSurviveOnSceneChange;
            ScreenAnchor = screenAnchor;
            Data = data;
            PopupOpenerAction = popupOpenerAction;
            QueueOpenTimeConditionEvaluate = queueOpenTimeConditionEvaluate;
            QueuePosition = queuePosition;
        }

        public PopupQueueItem(
            Func<PopupData> dataProvider,
            bool shouldSurviveOnSceneChange,
            Action popupOpenerAction,
            QueueOpenTimeConditionEvaluate queueOpenTimeConditionEvaluate,
            CFScreenAnchors screenAnchor,
            QueuePosition queuePosition)
        {
            ShouldSurviveOnSceneChange = shouldSurviveOnSceneChange;
            ScreenAnchor = screenAnchor;
            Data = null;
            DataProvider = dataProvider;
            PopupOpenerAction = popupOpenerAction;
            QueueOpenTimeConditionEvaluate = queueOpenTimeConditionEvaluate;
            QueuePosition = queuePosition;
        }

        public Func<PopupData> DataProvider { get; set; }

        public PopupData Data { get; }
        public QueueOpenTimeConditionEvaluate QueueOpenTimeConditionEvaluate { get; }
        public Action PopupOpenerAction { get; }
        public CFScreenAnchors ScreenAnchor { get; }
        public QueuePosition QueuePosition { get; }
    }
}