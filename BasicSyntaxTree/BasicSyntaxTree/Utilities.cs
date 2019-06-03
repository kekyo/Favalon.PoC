using System;
using System.Collections.Generic;

namespace BasicSyntaxTree
{
    public static class Utilities
    {
        public static IEnumerable<U> Collect<T, U>(this IEnumerable<T> enumerable, Func<T, U?> extractor)
            where U : class
        {
            foreach (var value in enumerable)
            {
                if (extractor(value) is U r)
                {
                    yield return r;
                }
            }
        }

        public static List<T> List<T>(params T[] values) =>
            new List<T>(values);

        public static Lazy<T> Lazy<T>(Func<T> factory) =>
            new Lazy<T>(factory);

        public static Dictionary<T, U> ToDictionary<T, U>(this IReadOnlyDictionary<T, U> dict)
        {
            var temp = new Dictionary<T, U>();
            foreach (var entry in dict)
            {
                temp.Add(entry.Key, entry.Value);
            }
            return temp;
        }
    }
}
