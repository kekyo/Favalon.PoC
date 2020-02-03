using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    internal static class Utilities
    {
        public static string Join(string separator, IEnumerable<string> values) =>
#if NET35
            string.Join(separator, values.ToArray());
#else
            string.Join(separator, values);
#endif

        public static IEnumerable<T> Traverse<T>(this T? value, Func<T, T?> next, bool includeFirst = true)
            where T : class
        {
            var current = value;

            if (includeFirst)
            {
                while (current != null)
                {
                    yield return current;
                    current = next(current);
                }
            }
            else if (current != null)
            {
                while (true)
                {
                    current = next(current);
                    if (current == null)
                    {
                        break;
                    }
                    yield return current;
                }
            }
        }

        public static IEnumerable<T> Collect<T>(this IEnumerable<T> enumerable, Func<T, T?> mapper)
            where T: class
        {
            foreach (var value in enumerable)
            {
                var v = mapper(value);
                if (v != null)
                {
                    yield return v;
                }
            }
        }

        public static void Deconstruct<T>(this IEnumerable<T> @this, out T[]? arr)
        {
            if (@this is T[] a)
            {
                arr = a;
            }
            else
            {
                arr = default;
            }
        }

        public static void Deconstruct<T>(this IEnumerable<T> @this, out T[]? arr, out int length)
        {
            if (@this is T[] a)
            {
                arr = a;
                length = a.Length;
            }
            else
            {
                arr = default;
                length = -1;
            }
        }

        public static void Deconstruct(this Term term, out bool? value)
        {
            if (term is IdentityTerm identity)
            {
                if (identity.Identity == "true")
                {
                    value = true;
                    return;
                }
                else if (identity.Identity == "false")
                {
                    value = false;
                    return;
                }
            }
            value = null;
        }
    }
}
