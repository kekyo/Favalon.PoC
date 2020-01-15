using Favalon.Contexts;
using System;
using System.Collections;
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

        public static string PrettyPrint(this Type type, PrettyPrintContext context) =>
            context.HigherOrderDetail switch
            {
                HigherOrderDetails.Full => type.FullName,
                _ => type.Name
            };

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
    }
}
