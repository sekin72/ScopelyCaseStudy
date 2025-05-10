using System;
using UnityEngine;

namespace CerberusFramework.Utilities.MonoBehaviourUtilities
{
    public class AnimationEventTriggerDetector : MonoBehaviour
    {
        public event Action<string> AnimEventOccured;

        public event Action<float> AnimEventOccuredWithFloat;

        public event Action AnimEventOccuredNonParam;

        public void OnAnimEvent(string eventKey)
        {
            AnimEventOccured?.Invoke(eventKey);
        }

        public void OnAnimEventNonParam()
        {
            AnimEventOccuredNonParam?.Invoke();
        }

        public void OnAnimEventWithFloat(float value)
        {
            AnimEventOccuredWithFloat?.Invoke(value);
        }
    }
}