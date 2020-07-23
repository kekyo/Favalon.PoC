using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalet.Internal
{
    internal static class ReflectionExtension
    {
        private static readonly Dictionary<Type, string> readableNames =
            new Dictionary<Type, string>
            {
                { typeof(void), "void" },
                { typeof(bool), "bool" },
                { typeof(byte), "byte" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(ushort), "ushort" },
                { typeof(int), "int" },
                { typeof(uint), "uint" },
                { typeof(long), "long" },
                { typeof(ulong), "ulong" },
                { typeof(float), "float" },
                { typeof(double), "double" },
                { typeof(decimal), "decimal" },
                { typeof(char), "char" },
                { typeof(string), "string" },
                { typeof(object), "object" },
            };

#if NETSTANDARD1_0
        public static bool IsAssignableFrom(this Type lhs, Type rhs) =>
            lhs.GetTypeInfo().IsAssignableFrom(rhs.GetTypeInfo());
#endif

        public static string GetReadableName(this Type type) =>
            readableNames.TryGetValue(type, out var name) ?
                name :
                type.FullName;

        public static string GetReadableName(this MethodBase method) =>
            $"{method.DeclaringType.GetReadableName()}.{method.Name}";
    }
}
