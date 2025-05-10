using System;
using System.Collections.Generic;
using System.Linq;

namespace CerberusFramework.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        {
            return RandomizeUsing(source, new Random());
        }

        public static IEnumerable<T> RandomizeUsing<T>(this IEnumerable<T> source, Random rnd)
        {
            return source.OrderBy(_ => rnd.Next());
        }
    }
}