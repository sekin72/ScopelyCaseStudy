using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CerberusFramework.Core.UI.Components
{
    public class SafeArea : MonoBehaviour
    {
        public EdgeInsets MinimumPadding = new EdgeInsets(50, 0, 0, 0);
        public EdgeInsets MaximumPadding = new EdgeInsets(2147483647, 2147483647, 0, 2147483647);

#if UNITY_EDITOR
        public int ScreenX;
        public int ScreenY;
#endif

        public RectTransform RectTransform;

        public void Initialize()
        {
            UpdateSafeArea();
        }

        private Rect GetAdjustSafeArea(Vector2 referenceResolution)
        {
            var safeArea = Screen.safeArea;

            safeArea.xMin = Mathf.Clamp(safeArea.xMin, MinimumPadding.Left, MaximumPadding.Left);
            safeArea.xMax = referenceResolution.x - Mathf.Clamp(
                referenceResolution.x - safeArea.xMax, MinimumPadding.Right, MaximumPadding.Right
            );

            safeArea.yMin = Mathf.Clamp(safeArea.yMin, MinimumPadding.Bottom, MaximumPadding.Bottom);
            safeArea.yMax = referenceResolution.y - Mathf.Clamp(
                referenceResolution.y - safeArea.yMax, MinimumPadding.Top, MaximumPadding.Top
            );

            return safeArea;
        }

        [Button("UpdateSafeArea")]
        public void UpdateSafeArea()
        {
            var currentResolution = Screen.currentResolution;

#if UNITY_EDITOR
            currentResolution = new Resolution
            {
                width = ScreenX / 2,
                height = ScreenY / 2
            };
#endif

            var referenceResolution = new Vector2(currentResolution.width, currentResolution.height);
            var safeArea = GetAdjustSafeArea(referenceResolution);

            var minAnchor = safeArea.position;
            var maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= referenceResolution.x;
            minAnchor.y /= referenceResolution.y;
            maxAnchor.x /= referenceResolution.x;
            maxAnchor.y /= referenceResolution.y;

            RectTransform.anchorMin = minAnchor;

            RectTransform.anchorMax = maxAnchor;

            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}