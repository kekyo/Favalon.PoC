////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace System.Reflection
{
    internal static class ReflectionExtension
    {
#if NETSTANDARD1_1
        public static bool IsPublic(this Type type) =>
            type.GetTypeInfo().IsPublic;

        public static bool IsPrimitive(this Type type) =>
            type.GetTypeInfo().IsPrimitive;

        public static bool IsClass(this Type type) =>
            type.GetTypeInfo().IsClass;

        public static bool IsInterface(this Type type) =>
            type.GetTypeInfo().IsInterface;

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

        private static T SelectMethod<T>(this IEnumerable<T> methods, bool excludeStatic, Type[] argumentTypes)
            where T : MethodBase =>
            methods.First(method =>
                (!excludeStatic || (excludeStatic && !method.IsStatic)) &&
                method.GetParameters() is ParameterInfo[] ps &&
                (ps.Length == argumentTypes.Length) &&
                ps.Zip(argumentTypes, (p, t) => (p.ParameterType, t)).
                All(entry => entry.ParameterType.IsConvertibleFrom(entry.t)));  // TODO: maybe mistakes from order dependant.

        public static MethodInfo GetDeclaredMethod(this Type type, string name, params Type[] argumentTypes) =>
            type.GetTypeInfo().GetDeclaredMethods(name).SelectMethod(false, argumentTypes);

        public static ConstructorInfo GetDeclaredConstructor(this Type type, params Type[] argumentTypes) =>
            type.GetTypeInfo().DeclaredConstructors.SelectMethod(true, argumentTypes);

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

        public static bool IsClass(this Type type) =>
            type.IsClass;

        public static bool IsInterface(this Type type) =>
            type.IsInterface;

        public static bool IsValueType(this Type type) =>
            type.IsValueType;

        public static bool IsGenericType(this Type type) =>
            type.IsGenericType;

        public static bool IsGenericTypeDefinition(this Type type) =>
            type.IsGenericTypeDefinition;

        public static Assembly GetAssembly(this Type type) =>
            type.Assembly;

        public static MethodInfo GetDeclaredMethod(this Type type, string name, params Type[] argumentTypes) =>
            type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly, null, argumentTypes, null);

        public static ConstructorInfo GetDeclaredConstructor(this Type type, params Type[] argumentTypes) =>
            type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, argumentTypes, null);

        public static ConstructorInfo[] GetDeclaredConstructors(this Type type) =>
            type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static MethodInfo[] GetDeclaredMethods(this Type type) =>
            type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public static PropertyInfo[] GetDeclaredProperties(this Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

        public static MemberInfo AsMemberInfo(this Type type) =>
            type;

        public static Type? AsType(this MemberInfo member) =>
            member as Type;
#endif
    }
}
