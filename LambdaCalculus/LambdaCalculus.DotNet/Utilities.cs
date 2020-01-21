using Favalon.TermContexts;
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

        public static bool IsTypeConstructor(Type type) =>
            type.IsGenericTypeDefinition() && (type.GetGenericArguments().Length == 1);

        public static string GetName(this MemberInfo member, bool containsGenericSignature = true)
        {
            var type = member.AsType();
            if (type is Type ? type.IsGenericParameter : false)
            {
                return type!.Name;
            }

            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (type, member)
            {
                case (Type _, _) when containsGenericSignature && type!.IsGenericType():
                    var gta = Utilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetName(ga)));
                    return $"{name}<{gta}>";

                case (_, MethodInfo method) when containsGenericSignature && method.IsGenericMethod:
                    var gma = Utilities.Join(
                        ",",
                        method.GetGenericArguments().Select(ga => GetName(ga)));
                    return $"{name}<{gma}>";

                default:
                    return $"{name}";
            }
        }

        public static string GetName(this Type type, bool containsGenericSignature = true) =>
            type.AsMemberInfo().GetName(containsGenericSignature);

        private static string Append(this string a, string b) =>
            a + b;

        public static string GetFullName(this MemberInfo member, bool containsGenericSignature = true)
        {
            var type = member.AsType();
            if (type is Type ? type.IsGenericParameter : false)
            {
                return type!.Name;
            }

            var parentNames = member.DeclaringType?.GetFullName().Append(".") ??
                type?.Namespace.Append(".") ??
                string.Empty;
            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (type, member)
            {
                case (Type _, _) when containsGenericSignature && type!.IsGenericType():
                    var gta = Utilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetFullName(ga)));
                    return $"{parentNames}{name}<{gta}>";

                case (_, MethodInfo method) when containsGenericSignature && method.IsGenericMethod:
                    var gma = Utilities.Join(
                        ",",
                        method.GetGenericArguments().Select(ga => GetFullName(ga)));
                    return $"{parentNames}{name}<{gma}>";

                default:
                    return $"{parentNames}{name}";
            }
        }

        public static string GetFullName(this Type type, bool containsGenericSignature = true) =>
            type.AsMemberInfo().GetFullName(containsGenericSignature);

        public static (Type, Type[]) GetDelegateSignature(Type delegateType)
        {
            Debug.Assert(typeof(Delegate).IsAssignableFrom(delegateType));

            var invoke = delegateType.GetMethod("Invoke");

            var parameters = invoke.GetParameters();
            return parameters.Length >= 1 ?
                (invoke.ReturnType, parameters.Select(parameter => parameter.ParameterType).ToArray()) :
                (invoke.ReturnType, TypeEx.EmptyTypes);
        }

        public static string PrettyPrint(this Type type, PrettyPrintContext context) =>
            context.HigherOrderDetail switch
            {
                HigherOrderDetails.Full => type.GetFullName(),
                _ => type.GetName()
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
