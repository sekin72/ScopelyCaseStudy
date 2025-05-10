using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using CerberusFramework.Core.Managers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CerberusFramework.Core.UI.Components
{
    public class Darkinator : MonoBehaviour
    {
        public Image BlackScreen;
        public CFButton OutsideButton;

        [Range(0, 1)] public float FadeStartPercentage = 0.5f;

        public float FadeDuration = 0.25f;

        private readonly LinkedList<DarkenedObject> _objectList = new LinkedList<DarkenedObject>();
        private Transform _defaultParent;

        private Tweener _fadeTweener;
        private Color _initialColor;
        private Action _onTapOutside;

        private PopupManager _popupManager;
        private GameObject _selfGameObject;
        private Transform _selfTransform;
        private RectTransform _rectTransform;

        public void OnDestroy()
        {
            HandlePreviousAnimation();
        }

        public void Initialize(PopupManager popupManager, Action onTapOutside)
        {
            _popupManager = popupManager;
            _selfGameObject = gameObject;
            _selfTransform = transform;
            _defaultParent = _selfTransform.parent;
            _rectTransform = GetComponent<RectTransform>();
            BlackScreen ??= GetComponent<Image>();
            OutsideButton ??= GetComponent<CFButton>();
            _initialColor = BlackScreen.color;
            _onTapOutside = onTapOutside;
            OutsideButton.onClick.AddListener(OnTapOutside);
        }

        private void OnTapOutside()
        {
            _onTapOutside?.Invoke();
        }

        public void AttachBlackScreen(string registerKey, Transform target)
        {
            if (_objectList.Any(o => o.RegisterKey == registerKey))
            {
                return;
            }

            _selfTransform.SetParent(_popupManager.Container.transform);
            _rectTransform.offsetMin = Vector2.zero;
            _rectTransform.offsetMax = Vector2.zero;

            _selfTransform.SetParent(target.parent);
            _selfTransform.SetSiblingIndex(Mathf.Clamp(target.GetSiblingIndex() - 1, 0, int.MaxValue));

            var popupObject = new DarkenedObject(registerKey, target);
            _objectList.AddFirst(popupObject);
            if (_objectList.Count == 1)
            {
                ShowBlackScreen();
            }
        }

        public void DetachBlackScreen(string registerKey)
        {
            if (_objectList.All(o => o.RegisterKey != registerKey))
            {
                return;
            }

            if (_objectList.First.Value.RegisterKey != registerKey)
            {
                var removedPopupObject = _objectList.First(o => o.RegisterKey == registerKey);
                _objectList.Remove(removedPopupObject);
                return;
            }

            _objectList.RemoveFirst();
            if (_objectList.Count == 0)
            {
                _selfTransform.SetParent(_defaultParent);
            }
            else
            {
                _selfTransform.SetParent(_objectList.First.Value.TargetTransform.parent);
                _selfTransform.SetSiblingIndex(
                    Mathf.Clamp(_objectList.First.Value.TargetTransform.GetSiblingIndex() - 1, 0, int.MaxValue)
                );
            }

            if (_objectList.Count == 0)
            {
                HideBlackScreen();
            }
        }

        private void ShowBlackScreen()
        {
            HandlePreviousAnimation();

            BlackScreen.color = new Color(_initialColor.r, _initialColor.g, _initialColor.b, FadeStartPercentage);
            _selfGameObject.SetActive(true);

            _fadeTweener = BlackScreen.DOFade(_initialColor.a, FadeDuration);
            _fadeTweener.OnKill(() => _fadeTweener = null);
        }

        private void HideBlackScreen()
        {
            HandlePreviousAnimation();

            _fadeTweener = BlackScreen.DOFade(0, FadeDuration);
            _fadeTweener.OnKill(
                () =>
                {
                    _selfGameObject.SetActive(false);
                    _fadeTweener = null;
                }
            );
        }

        private void HandlePreviousAnimation()
        {
            _fadeTweener?.Kill();
            _fadeTweener = null;
        }

        private readonly struct DarkenedObject : IEquatable<DarkenedObject>
        {
            public readonly string RegisterKey;
            public readonly Transform TargetTransform;

            public DarkenedObject(string registerKey, Transform targetTransform)
            {
                RegisterKey = registerKey;
                TargetTransform = targetTransform;
            }

            public bool Equals(DarkenedObject other)
            {
                return RegisterKey == other.RegisterKey;
            }
        }
    }
}