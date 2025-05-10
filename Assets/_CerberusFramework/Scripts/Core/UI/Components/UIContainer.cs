using System.Threading;
using Cysharp.Threading.Tasks;
using MessagePipe;
using CerberusFramework.Core.Events;
using CerberusFramework.Core.Managers.DeviceConfiguration;
using CerberusFramework.Core.UI.Screens;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CerberusFramework.Core.UI.Components
{
    public class UIContainer : MonoBehaviour
    {
        public ICFScreen CurrentScreen => ScreenGroup.CurrentScreen;

        [SerializeField] private ScreenGroup ScreenGroup;

        public Canvas Canvas;
        public CanvasScaler CanvasScaler;

        private IPublisher<UIContainerDestroyedEvent> _uiContainerDestroyedPublisher;

        private DeviceConfigurationManager _deviceConfigurationManager;

        [Inject]
        public void Inject(DeviceConfigurationManager deviceConfigurationManager)
        {
            _deviceConfigurationManager = deviceConfigurationManager;
        }

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            await ScreenGroup.Initialize(cancellationToken);
        }

        public async UniTask Activate(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);

            CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            CanvasScaler.matchWidthOrHeight = 0.5f;

            if (_deviceConfigurationManager.IsPC)
            {
                CanvasScaler.referenceResolution = new Vector2(1920, 1080);
            }
            else if (_deviceConfigurationManager.IsMobile)
            {
                CanvasScaler.referenceResolution = new Vector2(828, 1792);
            }

            await ScreenGroup.ActivateGradual(cancellationToken);

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Canvas.transform);
            _uiContainerDestroyedPublisher = GlobalMessagePipe.GetPublisher<UIContainerDestroyedEvent>();

            GlobalMessagePipe.GetPublisher<UIContainerCreatedEvent>().Publish(new UIContainerCreatedEvent(this));
        }

        public void Deactivate()
        {
            ScreenGroup.Deactivate();
            ScreenGroup.Dispose();
            gameObject.SetActive(false);
            _uiContainerDestroyedPublisher.Publish(new UIContainerDestroyedEvent(this));
        }
    }
}