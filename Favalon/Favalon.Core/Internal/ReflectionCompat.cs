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

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsPublic(this Type type) =>
            type.IsPublic;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNestedPublic(this Type type) =>
            type.IsNestedPublic;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsGenericType(this Type type) =>
            type.IsGenericType;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static MethodInfo[] GetMethods(this Type type) =>
            type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Deconstruct(this MemberInfo member, out Type? type) =>
            type = member as Type;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static MethodInfo GetMethodInfo(this Delegate dlg) =>
            dlg.Method;

#else   ///////////////////////////////////////////////////////////////

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Assembly GetAssembly(this Type type) =>
            type.GetAssembly();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetTypes(this Assembly assembly) =>
            assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType()).ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MemberInfo AsMemberInfo(this Type type) =>
            type.GetTypeInfo();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type? AsType(this MemberInfo member) =>
            member is TypeInfo typeInfo ? typeInfo.AsType() : null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPublic(this Type type) =>
            type.GetTypeInfo().IsPublic;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNestedPublic(this Type type) =>
            type.GetTypeInfo().IsNestedPublic;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericType(this Type type) =>
            type.GetTypeInfo().IsGenericType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo[] GetMethods(this Type type) =>
            type.GetTypeInfo().DeclaredMethods.ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetGenericArguments(this Type type) =>
            type.GenericTypeArguments;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Deconstruct(this MemberInfo member, out Type? type) =>
            type = member.AsType();
#endif
    }
}
