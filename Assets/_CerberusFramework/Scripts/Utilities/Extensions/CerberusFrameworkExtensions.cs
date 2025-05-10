using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CerberusFramework.Utilities.Extensions
{
    public static class CerberusFrameworkExtensions
    {
        private static readonly System.Random Rng = new System.Random();

        public static string ToFormattedString(this int value)
        {
            return value.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static Vector3 WorldToCanvasPosition(this Camera cam, Vector3 worldPos, RectTransform area)
        {
            var screenPoint = cam.WorldToScreenPoint(worldPos);
            screenPoint.z = 0;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPoint, cam, out var screenPos))
            {
                return screenPos;
            }

            return screenPoint;
        }

        public static Vector3 WorldToCanvasViewportPosition(this Camera cam, Vector3 worldPos, RectTransform area)
        {
            var screenPoint = cam.WorldToViewportPoint(worldPos);
            screenPoint.z = 0;

            var localPoint = Vector3.zero;
            localPoint.x = screenPoint.x * area.rect.width;
            localPoint.y = screenPoint.y * area.rect.height;

            return localPoint;
        }

        public static bool IsInsideGameScreen(this Camera cam, Vector3 worldPos)
        {
            var screenPoint = cam.WorldToScreenPoint(worldPos);
            screenPoint.z = 0;

            return screenPoint.x >= 0 && screenPoint.x <= Screen.width && screenPoint.y >= 0
                && screenPoint.y <= Screen.height;
        }

        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }

            var randomNum = Random.Range(0, list.Count);
            return list[randomNum];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static List<T> ShiftLeft<T>(this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange(shiftBy, list.Count - shiftBy);
            result.AddRange(list.GetRange(0, shiftBy));
            return result;
        }

        public static List<T> ShiftRight<T>(this List<T> list, int shiftBy)
        {
            if (list.Count <= shiftBy)
            {
                return list;
            }

            var result = list.GetRange(list.Count - shiftBy, shiftBy);
            result.AddRange(list.GetRange(0, list.Count - shiftBy));
            return result;
        }

        public static bool IsSame<T>(this IList<T> x, IList<T> y, bool isNullAndEmptySame = false)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (isNullAndEmptySame)
            {
                if (x == null)
                {
                    return y.Count == 0;
                }

                if (y == null)
                {
                    return x.Count == 0;
                }
            }
            else
            {
                if (y == null)
                {
                    return false;
                }

                if (x == null)
                {
                    return false;
                }
            }

            if (x.Count != y.Count)
            {
                return false;
            }

            foreach (var i in x)
            {
                if (!y.Contains(i))
                {
                    return false;
                }
            }

            return true;
        }

        public static Vector3 GetWorldCenter(this RectTransform r)
        {
            return r.TransformPoint(r.rect.center);
        }

        public static Vector3 GetWorldDimensions(this RectTransform r)
        {
            return r.TransformPoint(r.rect.max) - r.TransformPoint(r.rect.min);
        }
    }
}