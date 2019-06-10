using System;
using System.Collections;
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

        public static Dictionary<T, U> ToDictionary<T, U>(this IEnumerable<KeyValuePair<T, U>> dict)
        {
            var temp = (dict is ICollection coll) ? new Dictionary<T, U>(coll.Count) : new Dictionary<T, U>();
            foreach (var entry in dict)
            {
                temp.Add(entry.Key, entry.Value);
            }
            return temp;
        }

        public static Dictionary<T, U> ToDictionary<T, U>(this IEnumerable<(T key, U value)> dict)
        {
            var temp = (dict is ICollection coll) ? new Dictionary<T, U>(coll.Count) : new Dictionary<T, U>();
            foreach (var entry in dict)
            {
                temp.Add(entry.key, entry.value);
            }
            return temp;
        }
    }
}
