using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeInference
{
    public static class EnumerableExtensioncs
    {
        public static IEnumerable<U> Collect<T, U>(this IEnumerable<T> enumerable, Func<T, Option<U>> collector)
        {
            foreach (var value in enumerable)
            {
                if (collector(value).TryGetValue(out var v))
                {
                    yield return v;
                }
            }
        }
    }
}
