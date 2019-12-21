using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    internal static class Utilities
    {
        private static readonly HashSet<Type> knownClsCompilant =
            new HashSet<Type>
            {
                typeof(bool), typeof(byte), typeof(short), typeof(int), typeof(long),
                typeof(float), typeof(double),
            };
        private static readonly HashSet<Type> knownInteger =
            new HashSet<Type>
            {
                typeof(byte), typeof(short), typeof(int), typeof(long),
                typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong),
            };
        private static readonly Dictionary<Type, int> sizeOf =
            new Dictionary<Type, int>
            {
                { typeof(bool), 1 },
                { typeof(byte), 1 },
                { typeof(sbyte), 1 },
                { typeof(short), 2 },
                { typeof(ushort), 2 },
                { typeof(int), 4 },
                { typeof(uint), 4 },
                { typeof(long), 8 },
                { typeof(ulong), 8 },
                { typeof(float), 4 },
                { typeof(double), 8 },
            };

        public static int SizeOf(this Type type) =>
            sizeOf[type];

        public static bool IsClsCompliant(this Type type) =>
            knownClsCompilant.Contains(type);

        public static bool IsInteger(this Type type) =>
            knownInteger.Contains(type);

        public static (Type, Type[]) GetDelegateSignature(Type delegateType)
        {
            Debug.Assert(typeof(Delegate).IsAssignableFrom(delegateType));

            var invoke = delegateType.GetMethod("Invoke");

            var parameters = invoke.GetParameters();
            return parameters.Length >= 1 ?
                (invoke.ReturnType, parameters.Select(parameter => parameter.ParameterType).ToArray()) :
                (invoke.ReturnType, Type.EmptyTypes);
        }
    }
}

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
