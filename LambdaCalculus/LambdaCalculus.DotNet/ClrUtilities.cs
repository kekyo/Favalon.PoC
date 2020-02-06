using Favalon.Terms.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    internal enum NameOptions
    {
        WithGenericParameters,
        WithoutGenericParameters,
        Symbols,
    }

    internal static class ClrUtilities
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

        private static readonly Dictionary<string, string> operatorSymbols = new Dictionary<string, string>
        {
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Append", "+" },
            { "op_Concatenate", "+" },
            { "op_Modulus", "%" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_ExclusiveOr", "^" },
            { "op_LeftShift", "<<" },
            { "op_LogicalNot", "!" },
            { "op_RightShift", ">>" },
            { "op_UnaryPlus", "+" },
            { "op_UnaryNegation", "-" },
            { "op_Equality", "==" },
            { "op_Inequality", "!=" },
            { "op_LessThanOrEqual", "<=" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_LessThan", "<" },
            { "op_GreaterThan", ">" },
            { "op_PipeRight", "|>" },
            { "op_PipeLeft", "<|" },
            { "op_Dereference", "!" },
            { "op_ComposeRight", ">>" },
            { "op_ComposeLeft", "<<" },
            { "op_AdditionAssignment", "+=" },
            { "op_SubtractionAssignment", "-=" },
            { "op_MultiplyAssignment", "*=" },
            { "op_DivisionAssignment", "/=" },
        };

        public static int SizeOf(this Type type) =>
            sizeOf[type];

        public static bool IsClsCompliant(this Type type) =>
            knownClsCompilant.Contains(type);

        public static bool IsInteger(this Type type) =>
            knownInteger.Contains(type);

        public static bool IsTypeConstructor(Type type) =>
            type.IsGenericTypeDefinition() && (type.GetGenericArguments().Length == 1);

        public static string GetName(this MemberInfo member, NameOptions option = NameOptions.WithGenericParameters)
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

            if (member is MethodInfo method1 && method1.IsSpecialName)
            {
                if (option == NameOptions.Symbols &&
                    operatorSymbols.TryGetValue(name, out var symbol))
                {
                    name = symbol;
                }
                else if (method1.DeclaringType is Type type2 &&
                    type2.GetDeclaredProperties().FirstOrDefault(p =>
                        method1.Equals(p.GetGetMethod()) || method1.Equals(p.GetSetMethod())) is PropertyInfo property)
                {
                    name = property.Name;
                }

                // TODO: event
            }

            switch (type, member)
            {
                case (Type _, _) when (option == NameOptions.WithGenericParameters) && type!.IsGenericType():
                    var gta = Utilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetName(ga, option)));
                    return $"{name}<{gta}>";

                case (_, MethodInfo method2) when (option == NameOptions.WithGenericParameters) && method2.IsGenericMethod:
                    var gma = Utilities.Join(
                        ",",
                        method2.GetGenericArguments().Select(ga => GetName(ga, option)));
                    return $"{name}<{gma}>";

                default:
                    return $"{name}";
            }
        }

        public static string GetName(this Type type, NameOptions option = NameOptions.WithGenericParameters) =>
            type.AsMemberInfo().GetName(option);

        private static string Append(this string a, string b) =>
            a + b;

        public static string GetFullName(this MemberInfo member, bool withGenericParameters = true)
        {
            var type = member.AsType();
            if (type is Type ? type.IsGenericParameter : false)
            {
                return type!.Name;
            }

            var parentNames = member.DeclaringType?.GetFullName(withGenericParameters).Append(".") ??
                type?.Namespace.Append(".") ??
                string.Empty;
            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (type, member)
            {
                case (Type _, _) when withGenericParameters && type!.IsGenericType():
                    var gta = Utilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetFullName(ga, withGenericParameters)));
                    return $"{parentNames}{name}<{gta}>";

                case (_, MethodInfo method) when withGenericParameters && method.IsGenericMethod:
                    var gma = Utilities.Join(
                        ",",
                        method.GetGenericArguments().Select(ga => GetFullName(ga, withGenericParameters)));
                    return $"{parentNames}{name}<{gma}>";

                default:
                    return $"{parentNames}{name}";
            }
        }

        public static string GetFullName(this Type type, bool withGenericParameters = true) =>
            type.AsMemberInfo().GetFullName(withGenericParameters);

        public static (Type, Type[]) GetDelegateSignature(Type delegateType)
        {
            Debug.Assert(typeof(Delegate).IsAssignableFrom(delegateType));

            var invoke = delegateType.GetMethod("Invoke");

            var parameters = invoke.GetParameters();
            return parameters.Length >= 1 ?
                (invoke.ReturnType, parameters.Select(parameter => parameter.ParameterType).Memoize()) :
                (invoke.ReturnType, TypeEx.EmptyTypes);
        }

        public static string PrettyPrint(this Type type, PrettyPrintContext context, NameOptions option = NameOptions.WithGenericParameters) =>
            context.HigherOrderDetail switch
            {
                HigherOrderDetails.Full => type.GetFullName(option == NameOptions.WithGenericParameters),
                _ => type.GetName(option)
            };
    }
}
