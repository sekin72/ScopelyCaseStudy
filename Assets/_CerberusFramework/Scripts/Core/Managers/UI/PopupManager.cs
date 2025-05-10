using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Core.UI.Popups;
using CerberusFramework.Core.UI.Popups.Helpers;
using CerberusFramework.Utilities;
using CerberusFramework.Utilities.Logging;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using PoolManager = CerberusFramework.Core.Managers.Pool.PoolManager;

namespace CerberusFramework.Core.Managers.UI
{
    public sealed class PopupManager : Manager
    {
        private const string OpeningLockBinName = "OpeningPopupLockBin";
        private static readonly ICerberusLogger Logger = CerberusLogger.GetLogger(nameof(PopupManager));

        private readonly LinkedList<IPopup> _displayedPopups = new LinkedList<IPopup>();
        private readonly LockBin _popupQueueLockBin = new LockBin();
        private IDisposable _messageSubscriptions;
        private IPublisher<PopUpOpenedEvent> _popUpOpenedPublisher;
        private IPublisher<PopupClosedEvent> _popupClosedPublisher;
        private Queue<PopupQueueItem> _popupQueue = new Queue<PopupQueueItem>();
        public bool IsQueueLocked => (bool)_popupQueueLockBin;

        private PoolManager _poolManager;
        private IObjectResolver _resolver;

        public UIContainer Container { get; private set; }

        public override bool IsCore => true;

        public int State { get; private set; }

        [Inject]
        private void Inject(IObjectResolver objectResolver, PoolManager poolManager)
        {
            _resolver = objectResolver;
            _poolManager = poolManager;
        }

        protected override UniTask Initialize(CancellationToken disposeToken)
        {
            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<UIContainerCreatedEvent>().Subscribe(OnUIContainerCreated).AddTo(bagBuilder);
            GlobalMessagePipe.GetSubscriber<ScreenChangedEvent>().Subscribe(OnScreenChangedEvent).AddTo(bagBuilder);
            _messageSubscriptions = bagBuilder.Build();

            _popUpOpenedPublisher = GlobalMessagePipe.GetPublisher<PopUpOpenedEvent>();
            _popupClosedPublisher = GlobalMessagePipe.GetPublisher<PopupClosedEvent>();

            SetReady();

            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            ClearAll();
            _popupQueue.Clear();
            _messageSubscriptions.Dispose();
            base.Dispose();
        }

        private void OnUIContainerCreated(UIContainerCreatedEvent evt)
        {
            SetContextCanvas(evt.UiContainer);
        }

        private void OnScreenChangedEvent(ScreenChangedEvent evt)
        {
            SetContextCanvas(Container);
        }

        private void SetContextCanvas(UIContainer container)
        {
            Container = container;
            Container.CurrentScreen.Darkinator.Initialize(this, OnTapOutside);
        }

        private void OnTapOutside()
        {
            if (_displayedPopups.Count <= 0)
            {
                return;
            }

            var popup = _displayedPopups.Last.Value;
            popup.TapOutside();
        }

        public bool IsAnyPopUpOpened()
        {
            return _displayedPopups.Count > 0;
        }

        public IPopup GetPopup<T>()
            where T : IPopup
        {
            foreach (var popup in _displayedPopups)
            {
                if (popup is T)
                {
                    return popup;
                }
            }

            return null;
        }

        public void Queue<TP, TD, TV>(
            TD data,
            CancellationToken cancellationToken,
            bool shouldSurviveOnSceneChange = false,
            QueueOpenTimeConditionEvaluate condition = null,
            QueuePosition position = QueuePosition.AddLastOnInsert,
            CFScreenAnchors screenAnchor = CFScreenAnchors.Any)
            where TP : Popup<TD, TV>, new()
            where TD : PopupData
            where TV : PopupView
        {
            if (cancellationToken == CancellationToken.None)
            {
                cancellationToken = DisposeToken;
            }

            var popupQueueItem = new PopupQueueItem(
                data,
                shouldSurviveOnSceneChange,
                () =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    Open<TP, TD, TV>(data, PopupShowActions.HideAll, cancellationToken).Forget();
                },
                condition,
                screenAnchor,
                position
            );

            InsertQueueItem(position, popupQueueItem);
        }

        public void Queue<TP, TD, TV>(
            Func<TD> dataProvider,
            CancellationToken cancellationToken,
            bool shouldSurviveOnSceneChange = false,
            QueueOpenTimeConditionEvaluate condition = null,
            QueuePosition position = QueuePosition.AddLastOnInsert,
            CFScreenAnchors screenAnchor = CFScreenAnchors.Any)
            where TP : Popup<TD, TV>, new()
            where TD : PopupData
            where TV : PopupView
        {
            var popupQueueItem = new PopupQueueItem(
                dataProvider,
                shouldSurviveOnSceneChange,
                () =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    Open<TP, TD, TV>(dataProvider.Invoke(), PopupShowActions.HideAll, cancellationToken).Forget();
                },
                condition,
                screenAnchor,
                position
            );

            InsertQueueItem(position, popupQueueItem);
        }

        private void InsertQueueItem(
            QueuePosition position,
            PopupQueueItem popupQueueItem)
        {
            State++;
            if (position == QueuePosition.MoveToFrontOnInsertOnly)
            {
                var items = _popupQueue.ToArray();
                _popupQueue = new Queue<PopupQueueItem>();
                _popupQueue.Enqueue(
                    popupQueueItem
                );
                foreach (var item in items)
                {
                    _popupQueue.Enqueue(item);
                }
            }
            else
            {
                _popupQueue.Enqueue(
                    popupQueueItem
                );
            }

            ConsumeQueue();
        }

        public async UniTask Open<TP, TD, TV>(
            TD data,
            PopupShowActions popupShowAction,
            CancellationToken cancellationToken)
            where TP : Popup<TD, TV>, new()
            where TD : PopupData
            where TV : PopupView
        {
            State++;
            CFButton.DisableInput(OpeningLockBinName);
            PauseQueue(OpeningLockBinName);

            try
            {
                data.AttachCloseCall(Close);

                var viewPrefab = _poolManager.GetGameObject(data.PoolKey);
                var popupView = viewPrefab.GetComponent<TV>();
                var popup = _resolver.Resolve<TP>();

                _resolver.Inject(popupView);
                popupView.transform.SetParent(Container.CurrentScreen.SafeArea.transform, false);

                await popup.InitializeController(data, popupView, cancellationToken);
                popup.ActivateController();

                Logger.Debug($"{popup.UniqueName}");

                popupView.Root.rotation = Quaternion.identity;
                popupView.Root.localScale = Vector3.one;
                ((RectTransform)popupView.Root).anchorMin = Vector2.zero;
                ((RectTransform)popupView.Root).anchorMax = Vector2.one;
                ((RectTransform)popupView.Root).offsetMin = Vector2.zero;
                ((RectTransform)popupView.Root).offsetMax = Vector2.zero;

                switch (popupShowAction)
                {
                    case PopupShowActions.DoNothing:
                        if (data.ShowDarkinator)
                        {
                            Container.CurrentScreen.Darkinator.AttachBlackScreen(popup.UniqueName, popupView.transform);
                        }

                        break;

                    case PopupShowActions.CloseAll:
                        if (data.ShowDarkinator)
                        {
                            Container.CurrentScreen.Darkinator.AttachBlackScreen(popup.UniqueName, popupView.transform);
                        }

                        CloseAll();
                        break;

                    case PopupShowActions.HideAll:
                        HideAll();
                        if (data.ShowDarkinator)
                        {
                            Container.CurrentScreen.Darkinator.AttachBlackScreen(popup.UniqueName, popupView.transform);
                        }

                        break;
                }

                _displayedPopups.AddLast(popup);
                _popUpOpenedPublisher.Publish(new PopUpOpenedEvent(popup.UniqueName));

                await popup.ActivateGradual(cancellationToken);
            }
            finally
            {
                ResumeQueue(OpeningLockBinName);
                CFButton.EnableInput(OpeningLockBinName);
            }
        }

        private void OpenNextIfHiddenOrFromQueue()
        {
            if (_popupQueueLockBin)
            {
                return;
            }

            if (_displayedPopups.Count > 0)
            {
                var lastPopup = _displayedPopups.Last.Value;
                lastPopup.Activate();
                if (lastPopup.Data.ShowDarkinator)
                {
                    Container.CurrentScreen.Darkinator.AttachBlackScreen(
                        lastPopup.UniqueName,
                        lastPopup.View.transform
                    );
                }
            }
            else if (_popupQueue.Count > 0)
            {
                ConsumeQueue();
            }
        }

        private void ConsumeQueue()
        {
            if (_popupQueueLockBin || _popupQueue.Count == 0 || _displayedPopups.Count > 0)
            {
                return;
            }

            PopupHelpers.ReorderPopupsForDisplayedScreenIndex(_popupQueue);

            var candidate = _popupQueue.Peek();
            if (candidate.ScreenAnchor != CFScreenAnchors.Any)
            {
                return;
            }

            var nextPopup = _popupQueue.Dequeue();
            if (nextPopup == null)
            {
                return;
            }

            if (nextPopup.QueueOpenTimeConditionEvaluate == null)
            {
                nextPopup.PopupOpenerAction();
            }
            else
            {
                if (nextPopup.QueueOpenTimeConditionEvaluate())
                {
                    nextPopup.PopupOpenerAction();
                }
                else
                {
                    ConsumeQueue();
                }
            }
        }

        public void Close(IPopup basePopup)
        {
            State++;
            if (basePopup != null)
            {
                Logger.Debug($"{basePopup.UniqueName}");
                _displayedPopups.Remove(basePopup);
                if (basePopup.Data.ShowDarkinator)
                {
                    Container.CurrentScreen.Darkinator.DetachBlackScreen(basePopup.UniqueName);
                }

                basePopup.DeactivateController();

                _popupClosedPublisher.Publish(new PopupClosedEvent(basePopup.GetType(), basePopup.UniqueName));
                basePopup.DisposeController();
                _poolManager.SafeReleaseObject(basePopup.Data.PoolKey, basePopup.View.Root.gameObject);
            }

            OpenNextIfHiddenOrFromQueue();
        }

        public async UniTask Close(IPopup basePopup, CancellationToken cancellationToken)
        {
            State++;
            if (basePopup != null)
            {
                Logger.Debug($"{basePopup.UniqueName}");
                _displayedPopups.Remove(basePopup);
                if (basePopup.Data.ShowDarkinator)
                {
                    Container.CurrentScreen.Darkinator.DetachBlackScreen(basePopup.UniqueName);
                }

                await basePopup.DeactivateGradual(cancellationToken);
                basePopup.DeactivateController();

                _popupClosedPublisher.Publish(new PopupClosedEvent(basePopup.GetType(), basePopup.UniqueName));
                basePopup.DisposeController();
                _poolManager.SafeReleaseObject(basePopup.Data.PoolKey, basePopup.View.Root.gameObject);
            }

            OpenNextIfHiddenOrFromQueue();
        }

        public async UniTask Close(Type basePopupType, CancellationToken cancellationToken)
        {
            IPopup basePopup = null;
            foreach (var popup in _displayedPopups)
            {
                if (popup.GetType() != basePopupType)
                {
                    continue;
                }

                basePopup = popup;
                break;
            }

            if (basePopup != null)
            {
                await Close(basePopup, cancellationToken);
            }
        }

        public void CloseAll()
        {
            var tempPopups = ListPool<IPopup>.Get();
            tempPopups.AddRange(_displayedPopups);

            foreach (var basePopup in tempPopups)
            {
                Close(basePopup);
            }

            ListPool<IPopup>.Release(tempPopups);
            _displayedPopups.Clear();
        }

        public void ClearAll()
        {
            EmptyQueue();

            CloseAll();
        }

        public void EmptyQueue(bool keepSurviveOnSceneChange = true)
        {
            State++;
            var tempQueue = new Queue<PopupQueueItem>();

            if (keepSurviveOnSceneChange)
            {
                foreach (var queueItem in _popupQueue.Where(queueItem => queueItem.ShouldSurviveOnSceneChange))
                {
                    tempQueue.Enqueue(queueItem);
                }
            }

            _popupQueue = tempQueue;
        }

        private void HideAll(bool alsoHideDarkinator = false)
        {
            Logger.Debug("[PopUpManager] Hide All");
            foreach (var basePopup in _displayedPopups)
            {
                basePopup.Deactivate();
                if (alsoHideDarkinator && basePopup.Data.ShowDarkinator)
                {
                    Container.CurrentScreen.Darkinator.DetachBlackScreen(basePopup.UniqueName);
                }
            }
        }

        public void GoBack()
        {
            if (_displayedPopups.Count <= 0)
            {
                return;
            }

            var popup = _displayedPopups.Last.Value;
            popup.GoBack();
        }

        public void PauseQueue(string lockKey)
        {
            _popupQueueLockBin.Increase(lockKey);
        }

        public void ResumeQueue(string lockKey)
        {
            _popupQueueLockBin.Decrease(lockKey);
            ConsumeQueue();
        }

        public void HideAndPauseAllPopups()
        {
            PauseQueue("HideAndPauseAllPopups");
            HideAll(true);
        }

        public void ShowAndResumeAllPopups()
        {
            ResumeQueue("HideAndPauseAllPopups");
            OpenNextIfHiddenOrFromQueue();
        }
    }

    public delegate bool QueueOpenTimeConditionEvaluate();
}