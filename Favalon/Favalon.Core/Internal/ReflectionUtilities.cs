using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Internal
{
    internal static partial class ReflectionUtilities
    {
        public static string GetFullName(this MemberInfo member)
        {
            var parentNames = (member.DeclaringType as Type)?.GetFullName() ??
                member.AsType()?.Namespace ?? string.Empty;
            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (member)
            {
                case MemberInfo(Type type) when type.IsGenericType():
                    var gta = StringUtilities.Join(
                        ",",
                        type.GetGenericArguments().Select(GetFullName));
                    return $"{parentNames}.{name}<{gta}>";

                case MethodInfo method when method.IsGenericMethod:
                    var gma = StringUtilities.Join(
                        ",",
                        method.GetGenericArguments().Select(GetFullName));
                    return $"{parentNames}.{name}<{gma}>";

                default:
                    return $"{parentNames}.{name}";
            }
        }

        public static string GetFullName(this Type type) =>
            type.AsMemberInfo().GetFullName();

        public static string GetIdentity(this Delegate dlg) =>
            $"&{dlg.GetMethodInfo().GetFullName()}";
    }
}
