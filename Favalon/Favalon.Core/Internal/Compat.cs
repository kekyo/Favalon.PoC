using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Internal
{
    internal static class Compat
    {
#if NET35 || NET40 || NET45
        public static Assembly GetAssembly(this Type type) =>
            type.Assembly;

        public static MemberInfo AsMemberInfo(this Type type) =>
            type;

        public static Type? AsType(this MemberInfo member) =>
            member as Type;
#else
        public static Assembly GetAssembly(this Type type) =>
            type.GetTypeInfo().Assembly;

        public static TypeInfo[] GetTypes(this Assembly assembly) =>
            assembly.DefinedTypes.ToArray();

        public static MethodInfo[] GetMethods(this TypeInfo type) =>
            type.DeclaredMethods.ToArray();
        
        public static MemberInfo AsMemberInfo(this Type type) =>
            type.GetTypeInfo();

        public static TypeInfo? AsType(this MemberInfo member) =>
            member as TypeInfo;
#endif

#if NETSTANDARD1_0
        public static Type[] GetGenericArguments(this TypeInfo type) =>
            type.GenericTypeArguments;
#endif

#if NET35
        public static string Join(string separator, IEnumerable<string> values) =>
            string.Join(separator, values.ToArray());

        public static void Clear(this StringBuilder sb) =>
            sb.Length = 0;
#else
        public static string Join(string separator, IEnumerable<string> values) =>
            string.Join(separator, values);
#endif
    }
}
