using Favalon;
using Favalon.Terms.Contexts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace System
{
    public struct Unit
    {
        public static readonly Unit Value = new Unit();
    }
}

#if NET35
namespace System
{
    internal struct ValueTuple<T1, T2>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public ValueTuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
    }

    internal struct ValueTuple<T1, T2, T3>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;
        public readonly T3 Item3;

        public ValueTuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
    }
}

namespace System.Linq
{
    internal static class EnumerableExtension
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (var f = first.GetEnumerator())
            {
                using (var s = second.GetEnumerator())
                {
                    while (f.MoveNext() && s.MoveNext())
                    {
                        yield return resultSelector(f.Current, s.Current);
                    }
                }
            }
        }
    }
}
#endif

namespace System.Reflection
{
    internal static class TypeEx
    {
#if NETSTANDARD1_0
        public static readonly Type[] EmptyTypes =
            new Type[0];
#else
        public static Type[] EmptyTypes =>
            Type.EmptyTypes;
#endif
    }

    internal static class TypeExtension
    {
#if NETSTANDARD1_0
        public static bool IsPublic(this Type type) =>
            type.GetTypeInfo().IsPublic;

        public static bool IsPrimitive(this Type type) =>
            type.GetTypeInfo().IsPrimitive;

        public static bool IsValueType(this Type type) =>
            type.GetTypeInfo().IsValueType;

        public static bool IsGenericType(this Type type) =>
            type.GetTypeInfo().IsGenericType;

        public static bool IsGenericTypeDefinition(this Type type) =>
            type.GetTypeInfo().IsGenericTypeDefinition;

        public static Type[] GetGenericArguments(this Type type) =>
            type.GenericTypeArguments;

        public static bool IsAssignableFrom(this Type type, Type from) =>
            type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());

        public static Assembly GetAssembly(this Type type) =>
            type.GetTypeInfo().Assembly;

        public static MethodInfo GetMethod(this Type type, string name, params Type[] argumentTypes) =>
            type.GetRuntimeMethod(name, argumentTypes);

        public static IEnumerable<Type> GetTypes(this Assembly assembly) =>
            assembly.DefinedTypes.Select(typeInfo => typeInfo.AsType());

        public static IEnumerable<ConstructorInfo> GetDeclaredConstructors(this Type type) =>
            type.GetTypeInfo().DeclaredConstructors.Where(constructor => !constructor.IsStatic);

        public static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type) =>
            type.GetTypeInfo().DeclaredMethods.Where(method => method.IsPublic);

        public static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type) =>
            type.GetTypeInfo().DeclaredProperties;

        public static MethodInfo GetGetMethod(this PropertyInfo property) =>
            property.GetMethod;

        public static MethodInfo GetSetMethod(this PropertyInfo property) =>
            property.SetMethod;

        public static MemberInfo AsMemberInfo(this Type type) =>
            type.GetTypeInfo();

        public static Type? AsType(this MemberInfo member) =>
            member is TypeInfo typeInfo ? typeInfo.AsType() : null;
#else
        public static bool IsPublic(this Type type) =>
            type.IsPublic;

        public static bool IsPrimitive(this Type type) =>
            type.IsPrimitive;

        public static bool IsValueType(this Type type) =>
            type.IsValueType;

        public static bool IsGenericType(this Type type) =>
            type.IsGenericType;

        public static bool IsGenericTypeDefinition(this Type type) =>
            type.IsGenericTypeDefinition;
        
        public static Assembly GetAssembly(this Type type) =>
            type.Assembly;

        public static ConstructorInfo[] GetDeclaredConstructors(this Type type) =>
            type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static MethodInfo[] GetDeclaredMethods(this Type type) =>
            type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public static PropertyInfo[] GetDeclaredProperties(this Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public static MemberInfo AsMemberInfo(this Type type) =>
            type;

        public static Type? AsType(this MemberInfo member) =>
            member as Type;
#endif
    }
}
