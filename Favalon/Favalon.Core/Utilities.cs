using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    internal static class Utilities
    {
        public static IEnumerable<T> Traverse<T>(this T target, Func<T, T> next)
            where T : class
        {
            var current = target;
            while (current != null)
            {
                yield return current;
                current = next(current);
            }
        }
    }
}
