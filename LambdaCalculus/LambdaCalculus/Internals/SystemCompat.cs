using Favalon.Terms.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System
{
    public struct Unit
    {
        public static readonly Unit Value = new Unit();
    }
}

#if NET35
namespace System
{
    internal struct ValueTuple<T1, T2>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
    }

    internal struct ValueTuple<T1, T2, T3>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;
        public readonly T3 Item3;

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
    }
}

namespace System.Linq
{
    internal static class EnumerableExtension
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (var f = first.GetEnumerator())
            {
                using (var s = second.GetEnumerator())
                {
                    while (f.MoveNext() && s.MoveNext())
                    {
                        yield return resultSelector(f.Current, s.Current);
                    }
                }
            }
        }
    }
}
#endif
