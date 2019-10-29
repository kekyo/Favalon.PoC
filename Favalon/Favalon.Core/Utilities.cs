using System;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    internal static class Utilities
    {
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
            GetFullName(type.GetTypeInfo());
    }
}
