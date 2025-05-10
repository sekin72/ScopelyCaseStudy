using CerberusFramework.Core.Managers.Vibration;
using CerberusFramework.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CerberusFramework.Core.UI.Components
{
    [AddComponentMenu("UI/CF/CFButton")]
    public class CFButton : Button
    {
        public static readonly LockBin IsInputLocked = new LockBin();

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (IsInputLocked)
            {
                Debug.LogWarning("Input is Locked");
                return;
            }

            base.OnPointerDown(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (IsInputLocked)
            {
                return;
            }

            base.OnPointerClick(eventData);

            VibrationManager.VibrateStatic(VibrationType.LightImpact);
        }

        public static void DisableInput(string lockKey)
        {
            IsInputLocked.Increase(lockKey);
        }

        public static void EnableInput(string lockKey)
        {
            IsInputLocked.Decrease(lockKey);
        }
    }
}