using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Internal
{
    internal static class Utilities
    {
        public static IEnumerable<MethodInfo> EnumerableAllPublicStaticMethods(this Assembly assembly) =>
            assembly.GetTypes().
            Where(type => (type.IsPublic || type.IsNestedPublic) && !type.IsGenericType).
            SelectMany(type => type.GetMethods().Where(method => method.IsPublic && method.IsStatic && !method.IsGenericMethod));

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
#if NET35 || NET40 || NET45
                case Type type when type.IsGenericType:
#else
                case TypeInfo type when type.IsGenericType:
#endif
                    var gta = Compat.Join(
                        ",",
                        type.GetGenericArguments().Select(GetFullName));
                    return $"{parentNames}.{name}<{gta}>";

                case MethodInfo method when method.IsGenericMethod:
                    var gma = Compat.Join(
                        ",",
                        method.GetGenericArguments().Select(GetFullName));
                    return $"{parentNames}.{name}<{gma}>";

                default:
                    return $"{parentNames}.{name}";
            }
        }

        public static string GetFullName(this Type type) =>
            type.AsMemberInfo().GetFullName();

#if NETSTANDARD1_0
        public static string GetIdentity(this Delegate dlg) =>
            $"&{dlg.GetHashCode()}";
#else
        public static string GetIdentity(this Delegate dlg) =>
            $"&{dlg.Method.GetFullName()}";
#endif

        public static Term CombineTerms(Term? left, Term? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return new ApplyTerm(left, right);
                }
                else
                {
                    return left;
                }
            }
            else
            {
                return right!;
            }
        }

        public static Term CombineTerms(params Term?[] terms) =>
            terms.Aggregate(CombineTerms)!;

        public static ConstantTerm GetNumericConstant(string value, Signes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);
    }
}
