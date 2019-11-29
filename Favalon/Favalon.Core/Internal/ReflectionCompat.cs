using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Favalon.Internal
{
    partial class ReflectionUtilities
    {
#if NET35 || NET40 || NET45
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Assembly GetAssembly(this Type type) =>
            type.Assembly;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static MemberInfo AsMemberInfo(this Type type) =>
            type;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Type? AsType(this MemberInfo member) =>
            member as Type;
#else
#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Assembly GetAssembly(this Type type) =>
            type.GetTypeInfo().Assembly;

#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypeInfo[] GetTypes(this Assembly assembly) =>
            assembly.DefinedTypes.ToArray();

#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static MethodInfo[] GetMethods(this TypeInfo type) =>
            type.DeclaredMethods.ToArray();

#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static MemberInfo AsMemberInfo(this Type type) =>
            type.GetTypeInfo();

#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TypeInfo? AsType(this MemberInfo member) =>
            member as TypeInfo;
#endif

#if NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetGenericArguments(this TypeInfo type) =>
            type.GenericTypeArguments;
#endif
    }
}
