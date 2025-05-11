using System;
using CerberusFramework.Core.Managers.Pool;
using CerberusFramework.Core.UI.Components;
using CerberusFramework.Utilities.MonoBehaviourUtilities;
using MessagePipe;
using ScopelyCaseStudy.Core.Gameplay.Events;
using UnityEngine;
using UnityEngine.UI;

namespace ScopelyCaseStudy.Core.Gameplay.UI
{
    public class TurretSeller : MonoBehaviour
    {
        [SerializeField] private EasyInputManager _easyInputManager;
        [SerializeField] private GameObject _movedItem;

        [SerializeField] private Image _costImage;
        [SerializeField] private CFText _costText;

        private IPublisher<TurretPlacedEvent> _turretPlacedEventPublisher;

        private Vector2 _movedItemStartPosition;
        private PoolKeys _poolKey;
        private int _cost;

        private IDisposable _messageSubscription;

        private Color _onColor;
        private Color _offColor;

        private bool _isActive;

        private Camera _camera;

        private int _layerMask;

        public void Initialize(Camera camera, int cost, PoolKeys poolKey)
        {
            _camera = camera;
            _cost = cost;
            _poolKey = poolKey;

            var bagBuilder = DisposableBag.CreateBuilder();
            GlobalMessagePipe.GetSubscriber<GoldChangedEvent>().Subscribe(OnGoldChangedEvent).AddTo(bagBuilder);
            GlobalMessagePipe.GetSubscriber<TurretCostChangedEvent>().Subscribe(OnTurretCostChangedEvent).AddTo(bagBuilder);
            _messageSubscription = bagBuilder.Build();

            _turretPlacedEventPublisher = GlobalMessagePipe.GetPublisher<TurretPlacedEvent>();

            _onColor = _costImage.color;
            _offColor = Color.gray;
            _costText.Text = _cost.ToString();

            _movedItemStartPosition = _movedItem.transform.position;
            _movedItem.SetActive(false);

            _isActive = true;

            _layerMask = LayerMask.GetMask("Ground");

            _easyInputManager.Selected += OnSelected;
            _easyInputManager.Dragged += OnDragged;
            _easyInputManager.Released += OnReleased;
        }

        public void Dispose()
        {
            _messageSubscription?.Dispose();

            _easyInputManager.Selected -= OnSelected;
            _easyInputManager.Dragged -= OnDragged;
            _easyInputManager.Released -= OnReleased;
        }

        private void OnSelected(UnityEngine.EventSystems.PointerEventData obj)
        {
            if (!_isActive)
            {
                return;
            }

            _movedItem.transform.position = obj.position;
            _movedItem.SetActive(true);
        }

        private void OnDragged(UnityEngine.EventSystems.PointerEventData obj)
        {
            if (!_isActive)
            {
                return;
            }

            _movedItem.transform.position = obj.position;
        }

        private void OnReleased(UnityEngine.EventSystems.PointerEventData obj)
        {
            if (!_isActive)
            {
                return;
            }

            _movedItem.transform.position = _movedItemStartPosition;
            _movedItem.SetActive(false);

            Ray ray = _camera.ScreenPointToRay(obj.position);
            if (Physics.Raycast(ray, out var hit, 100f, _layerMask))
            {
                _turretPlacedEventPublisher.Publish(new TurretPlacedEvent(_poolKey, hit.point));
            }
        }

        private void OnGoldChangedEvent(GoldChangedEvent evt)
        {
            _isActive = evt.Gold >= _cost;
            SetCostImageColor(_isActive);
        }

        private void OnTurretCostChangedEvent(TurretCostChangedEvent evt)
        {
            if (_poolKey != evt.PoolKey)
            {
                return;
            }

            _cost = evt.Cost;
            _costText.Text = _cost.ToString();
        }

        private void SetCostImageColor(bool isActive)
        {
            _costImage.color = isActive ? _onColor : _offColor;
        }
    }
}
