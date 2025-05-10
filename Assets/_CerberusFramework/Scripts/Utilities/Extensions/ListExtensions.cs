using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;

namespace CerberusFramework.Utilities.Extensions
{
    public static class ListExtensions
    {
        public static void DoForAll<T>(this IList<T> list, Action<T> action)
        {
            var count = list.Count;

            for (var n = 0; n < count; n++)
            {
                var item = list[n];
                action(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> ShallowClone<T>(this IList<T> list)
        {
            list ??= new List<T>();

            return new List<T>(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> FilterList<T>(this List<T> list, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var t in list)
            {
                if (predicate(t))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public static List<T> GetRandomElements<T>(this List<T> list, int count)
        {
            if (count == 0)
            {
                return new List<T>(0);
            }

            if (count > list.Count)
            {
                var listCopy = list.ShallowClone();
                listCopy.Shuffle();
                return listCopy;
            }

            var selections = new List<T>(count);
            GetRandomElements(list, selections, count);
            return selections;
        }

        public static int GetRandomElements<T>(this List<T> list, List<T> selections, int count)
        {
            if (count == 0)
            {
                return 0;
            }

            var oldCount = selections.Count;
            if (count > list.Count)
            {
                using var po1 = ListPool<T>.Get(out var listCopy);
                listCopy.AddRange(list);
                listCopy.Shuffle();
                selections.AddRange(listCopy);
                return oldCount - selections.Count;
            }

            using var po2 = ListPool<T>.Get(out var temp);
            if (temp.Capacity < list.Count)
            {
                temp.Capacity = list.Count;
            }

            temp.AddRange(list);
            temp.Shuffle();

            for (var i = 0; i < count; i++)
            {
                selections.Add(temp[i]);
            }

            return oldCount - selections.Count;
        }

        public static bool IsNullOrEmpty(this ICollection source)
        {
            return source == null || source.Count < 1;
        }
    }
}