using System;
using System.Reflection;

namespace Favalet.Internal
{
    internal static class Reflection
    {
#if NETSTANDARD1_0
        public static bool IsAssignableFrom(this Type lhs, Type rhs) =>
            lhs.GetTypeInfo().IsAssignableFrom(rhs.GetTypeInfo());
#endif

        public static string GetReadableName(this Type type) =>
            type.FullName;

        public static string GetReadableName(this MethodBase method) =>
            $"{method.DeclaringType.GetReadableName()}.{method.Name}";
    }
}
