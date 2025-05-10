using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace CerberusFramework.Utilities.Extensions
{
    public static class DictionaryExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TKey> ToKeyList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new List<TKey>(dictionary.Keys);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TValue> ToValueList<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            return new List<TValue>(dictionary.Values);
        }

        public static KeyValuePair<TKey, TValue> GetElementAt<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary, int index)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (index < 0 || index >= dictionary.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            foreach (var pair in dictionary)
            {
                if (index == 0)
                {
                    return pair;
                }

                index--;
            }

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static KeyValuePair<TKey, TValue> GetRandomElement<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary)
        {
            var selected = Random.Range(0, dictionary.Count);
            return dictionary.GetElementAt(selected);
        }

        public static KeyValuePair<TKey, TValue> GetRandomElement<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> filter)
        {
            using var po1 = ListPool<int>.Get(out var validIndexes);
            var i = 0;
            foreach (var pair in dictionary)
            {
                if (filter(pair))
                {
                    validIndexes.Add(i);
                }

                i++;
            }

            if (validIndexes.Count == 0)
            {
                throw new Exception("No valid elements in dictionary");
            }

            var randomIndex = validIndexes.GetRandomElement();

            return dictionary.GetElementAt(randomIndex);
        }

        public static bool IsSame<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
        {
            if (dictionary.Count != other.Count)
            {
                return false;
            }

            foreach (var pair in dictionary)
            {
                if (!other.TryGetValue(pair.Key, out var value))
                {
                    return false;
                }

                if (!value.Equals(pair.Value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}