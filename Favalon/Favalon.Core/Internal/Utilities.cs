using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Internal
{
    internal static class Utilities
    {
        private static readonly HashSet<char> operatorChars = new HashSet<char>
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''' */, '(', ')',
            '*', '+', ',', '-'/* , '.'*/, '/'/*, ':' */, ';', '<', '=', '>', '?',
            '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
        };

        public static bool IsOperator(char ch) =>
            operatorChars.Contains(ch);

        public static IEnumerable<MethodInfo> EnumerableAllPublicStaticMethods(this Assembly assembly) =>
            assembly.DefinedTypes.
            Where(type => (type.IsPublic || type.IsNestedPublic) && !type.IsGenericType).
            SelectMany(type => type.DeclaredMethods.Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod));

        public static string GetFullName(this MemberInfo member)
        {
            var parentNames = (member.DeclaringType as Type)?.GetFullName() ??
                (member as TypeInfo)?.Namespace ?? string.Empty;
            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (member)
            {
                case TypeInfo type when type.IsGenericType:
                    var gta = string.Join(
                        ",",
                        type.GenericTypeArguments.Select(GetFullName));
                    return $"{parentNames}.{name}<{gta}>";

                case MethodInfo method when method.IsGenericMethod:
                    var gma = string.Join(
                        ",",
                        method.GetGenericArguments().Select(GetFullName));
                    return $"{parentNames}.{name}<{gma}>";

                default:
                    return $"{parentNames}.{name}";
            }
        }

        public static string GetFullName(this Type type) =>
            ((MemberInfo)type.GetTypeInfo()).GetFullName();
    }
}
