////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq
{
    internal static class EnumerableExtension
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] Memoize<T>(this IEnumerable<T> enumerable) =>
            enumerable as T[] ??
            (enumerable is List<T> list ? list.ToArray() : enumerable.ToArray());

        public static IEnumerable<U> Collect<T, U>(this IEnumerable<T> enumerable, Func<T, U> selector)
            where U : class
        {
            foreach (var value in enumerable)
            {
                if (selector(value) is U result)
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<T> Traverse<T>(this T value, Func<T, T?> next)
            where T : class
        {
            T? current = value;
            while (current != null)
            {
                yield return current;
                current = next(current);
            }
        }

#if NET35
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
#endif

#if !NETSTANDARD2_0 && !NETSTANDARD2_1
        public static IEnumerable<T> Append<T>(this IEnumerable<T> enumerable, T appendValue)
        {
            foreach (var value in enumerable)
            {
                yield return value;
            }
            yield return appendValue;
        }
#endif
    }
}
