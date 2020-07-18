using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Internal
{
    internal static class Enumerable
    {
        public static T[] Memoize<T>(
            this IEnumerable<T> enumerable) =>
            enumerable switch
            {
                T[] array => array,
                _ => enumerable.ToArray()
            };

        // unfold
        public static IEnumerable<U> Traverse<T, U>(
            this T? seed,
            Func<T, U?> predicate)
            where T : class
            where U : class, T
        {
            U? value = seed as U;
            while (value != null)
            {
                yield return value;
                value = predicate(value);
            }
        }

        public static IEnumerable<U> Collect<T, U>(
            this IEnumerable<T> enumerable,
            Func<T, U?> predicate)
            where U : class
        {
            foreach (var value in enumerable)
            {
                if (predicate(value) is U v)
                {
                    yield return v;
                }
            }
        }

#if NET40 || NET45 || NETSTANDARD1_0
        public static IEnumerable<T> Append<T>(
            this IEnumerable<T> enumerable,
            T value)
        {
            foreach (var v in enumerable)
            {
                yield return v;
            }
            yield return value;
        }
#endif

        public static bool EqualsPartiallyOrdered<T>(
            this IEnumerable<T> lhs,
            IEnumerable<T> rhs)
        {
            var l = new HashSet<T>(lhs);
            var r = new HashSet<T>(rhs);
            return lhs.All(r.Contains) && rhs.All(l.Contains);
        }
    }
}
