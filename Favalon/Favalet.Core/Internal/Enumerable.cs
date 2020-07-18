using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Internal
{
    internal static class Enumerable
    {
        // unfold
        public static IEnumerable<U> Traverse<T, U>(this T? seed, Func<T, U?> predicate)
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

        public static bool EqualsPartiallyOrdered<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs)
        {
            var l = new HashSet<T>(lhs);
            var r = new HashSet<T>(rhs);
            return lhs.All(r.Contains) && rhs.All(l.Contains);
        }
    }
}
