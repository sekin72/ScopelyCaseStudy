using System;
using UnityEngine.EventSystems;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class EasyInputManager : PlayerInputBase, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<PointerEventData> Selected;

        public event Action<PointerEventData> Dragged;

        public event Action<PointerEventData> Released;

        public event Action<PointerEventData> Entered;

        public event Action<PointerEventData> Exited;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Entered?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exited?.Invoke(eventData);
        }

        protected override void OnMultitouchOtherFingersDragged(PointerEventData pointerEventData)
        {
        }

        protected override void OnObjectDragged(PointerEventData pointerEventData)
        {
            Dragged?.Invoke(pointerEventData);
        }

        protected override void OnObjectSelected(PointerEventData pointerEventData)
        {
            Selected?.Invoke(pointerEventData);
        }

        protected override void OnObjectReleased(PointerEventData pointerEventData)
        {
            Released?.Invoke(pointerEventData);
        }

        protected override void OnFingerChanged(PointerEventData oldFingerData, int newFingerID)
        {
        }

        protected override void InitOnActivate()
        {
            MultiTouchSupport = true;
        }
    }
}