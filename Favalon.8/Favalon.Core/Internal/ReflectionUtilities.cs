using System;
using System.Linq;
using System.Reflection;

namespace Favalon.Internal
{
    internal static class ReflectionUtilities
    {
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

            switch (member)
            {
                case MemberInfo(Type _) when containsGenericSignature && type!.IsGenericType():
                    var gta = StringUtilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetFullName(ga)));
                    return $"{parentNames}{name}<{gta}>";

                case MethodInfo method when containsGenericSignature && method.IsGenericMethod:
                    var gma = StringUtilities.Join(
                        ",",
                        method.GetGenericArguments().Select(ga => GetFullName(ga)));
                    return $"{parentNames}{name}<{gma}>";

                default:
                    return $"{parentNames}{name}";
            }
        }

        public static string GetFullName(this Type type, bool containsGenericSignature = true) =>
            type.AsMemberInfo().GetFullName(containsGenericSignature);

        public static string GetIdentity(this Delegate dlg) =>
            $"&{dlg.GetMethodInfo().GetFullName()}";
    }
}
