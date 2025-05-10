using System;
using UnityEngine.EventSystems;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class ExtendedInputEventHandler : InputEventHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<PointerEventData> PointerEntered;

        public event Action<PointerEventData> PointerExited;

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEntered?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExited?.Invoke(eventData);
        }
    }
}