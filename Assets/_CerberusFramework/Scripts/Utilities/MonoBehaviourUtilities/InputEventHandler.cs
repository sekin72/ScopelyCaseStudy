using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class InputEventHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public event Action<PointerEventData> PointerDowned;

        public event Action<PointerEventData> PointerDragged;

        public event Action<PointerEventData> PointerUpped;

        public static event Action<PointerEventData> PointerDownStatic;

        public static event Action<PointerEventData> PointerUpStatic;

        public void OnDrag(PointerEventData eventData)
        {
            PointerDragged?.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDowned?.Invoke(eventData);
            PointerDownStatic?.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUpped?.Invoke(eventData);
            PointerUpStatic?.Invoke(eventData);
        }
    }
}