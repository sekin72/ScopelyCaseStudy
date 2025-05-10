using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    [RequireComponent(typeof(InputEventHandler))]
    public abstract class PlayerInputBase : MonoBehaviour
    {
        private PointerEventData _lastData;
        private List<int> _touchIdList;
        private int _touchingInputId = -1;
        protected InputEventHandler InputEventHandler;
        protected bool MultiTouchSupport = false;

        public virtual void Awake()
        {
            if (!TryGetComponent(out InputEventHandler))
            {
                InputEventHandler = gameObject.AddComponent<InputEventHandler>();
            }

            _touchIdList = new List<int>();
        }

        public virtual void Start()
        {
            InputEventHandler.PointerDowned += _OnObjectSelected;
            InputEventHandler.PointerDragged += _OnObjectDragged;
            InputEventHandler.PointerUpped += _OnObjectReleased;

            ActivateController();
        }

        [UsedImplicitly]
        public void OnApplicationFocus(bool hasFocus)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer)
            {
                return;
            }

            _touchIdList = new List<int>();
            if (!hasFocus && _touchingInputId != -1)
            {
                OnObjectReleased(_lastData);
            }

            _touchingInputId = -1;
        }

        protected abstract void OnMultitouchOtherFingersDragged(PointerEventData pointerEventData);

        protected abstract void OnObjectDragged(PointerEventData pointerEventData);

        protected abstract void OnObjectSelected(PointerEventData pointerEventData);

        protected abstract void OnObjectReleased(PointerEventData pointerEventData);

        protected abstract void OnFingerChanged(PointerEventData oldFingerData, int newFingerID);

        protected abstract void InitOnActivate();

        protected virtual void ActivateController()
        {
            InputEventHandler.enabled = true;
            InitOnActivate();
        }

        protected virtual void DeactivateController()
        {
            InputEventHandler.enabled = false;
        }

        private void _OnObjectSelected(PointerEventData pointerEventData)
        {
            _lastData = pointerEventData;

            if (!MultiTouchSupport)
            {
                if (_touchingInputId != -1)
                {
                    return;
                }

                OnObjectSelected(pointerEventData);
                _touchingInputId = pointerEventData.pointerId;
            }
            else
            {
                _touchIdList.Add(pointerEventData.pointerId);

                if (_touchingInputId == -1)
                {
                    _touchingInputId = pointerEventData.pointerId;
                }

                if (_touchingInputId == pointerEventData.pointerId)
                {
                    OnObjectSelected(pointerEventData);
                }
            }
        }

        private void _OnObjectDragged(PointerEventData pointerEventData)
        {
            _lastData = pointerEventData;

            if (!MultiTouchSupport)
            {
                if (pointerEventData.pointerId == _touchingInputId)
                {
                    OnObjectDragged(pointerEventData);
                }
            }
            else
            {
                if (pointerEventData.pointerId == _touchingInputId)
                {
                    OnObjectDragged(pointerEventData);
                }
                else
                {
                    OnMultitouchOtherFingersDragged(pointerEventData);
                }
            }
        }

        private void _OnObjectReleased(PointerEventData pointerEventData)
        {
            _lastData = pointerEventData;

            if (!MultiTouchSupport)
            {
                if (pointerEventData.pointerId != _touchingInputId)
                {
                    return;
                }

                _touchingInputId = -1;
                OnObjectReleased(pointerEventData);
            }
            else
            {
                if (_touchingInputId != pointerEventData.pointerId)
                {
                    _touchIdList.Remove(pointerEventData.pointerId);
                    return;
                }

                _touchIdList.Remove(_touchingInputId);

                if (_touchIdList.Count > 0)
                {
                    _touchingInputId = _touchIdList[0];
                    OnFingerChanged(pointerEventData, _touchingInputId);
                    return;
                }

                _touchingInputId = -1;
                OnObjectReleased(pointerEventData);
            }
        }
    }
}