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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Favalet.Internal
{
    internal enum NameOptions
    {
        WithGenericParameters,
        WithoutGenericParameters,
        Symbols,
    }

    internal static class ReflectionUtilities
    {
        private static readonly HashSet<Type> knownClsCompilant =
            new HashSet<Type>
            {
                typeof(bool), typeof(byte), typeof(short), typeof(int), typeof(long),
                typeof(float), typeof(double),
            };

        private static readonly HashSet<Type> knownInteger =
            new HashSet<Type>
            {
                typeof(byte), typeof(short), typeof(int), typeof(long),
                typeof(sbyte), typeof(ushort), typeof(uint), typeof(ulong),
            };

        private static readonly HashSet<(Type, Type)> primitiveConvertibles =
            new HashSet<(Type, Type)>
            {
                (typeof(char), typeof(ushort)),
                (typeof(char), typeof(int)),
                (typeof(char), typeof(uint)),
                (typeof(char), typeof(long)),
                (typeof(char), typeof(ulong)),
                (typeof(char), typeof(float)),
                (typeof(char), typeof(double)),
                (typeof(byte), typeof(char)),
                (typeof(byte), typeof(short)),
                (typeof(byte), typeof(ushort)),
                (typeof(byte), typeof(int)),
                (typeof(byte), typeof(uint)),
                (typeof(byte), typeof(long)),
                (typeof(byte), typeof(ulong)),
                (typeof(byte), typeof(float)),
                (typeof(byte), typeof(double)),
                (typeof(sbyte), typeof(short)),
                (typeof(sbyte), typeof(int)),
                (typeof(sbyte), typeof(long)),
                (typeof(sbyte), typeof(float)),
                (typeof(sbyte), typeof(double)),
                (typeof(short), typeof(int)),
                (typeof(short), typeof(long)),
                (typeof(short), typeof(float)),
                (typeof(short), typeof(double)),
                (typeof(ushort), typeof(int)),
                (typeof(ushort), typeof(uint)),
                (typeof(ushort), typeof(long)),
                (typeof(ushort), typeof(ulong)),
                (typeof(ushort), typeof(float)),
                (typeof(ushort), typeof(double)),
                (typeof(int), typeof(long)),
                (typeof(int), typeof(float)),
                (typeof(int), typeof(double)),
                (typeof(uint), typeof(long)),
                (typeof(uint), typeof(ulong)),
                (typeof(uint), typeof(float)),
                (typeof(uint), typeof(double)),
                (typeof(long), typeof(float)),
                (typeof(long), typeof(double)),
                (typeof(ulong), typeof(float)),
                (typeof(ulong), typeof(double)),
                (typeof(float), typeof(double)),
            };

        private static readonly Dictionary<Type, int> sizeOf =
            new Dictionary<Type, int>
            {
                { typeof(bool), 1 },
                { typeof(byte), 1 },
                { typeof(sbyte), 1 },
                { typeof(short), 2 },
                { typeof(ushort), 2 },
                { typeof(int), 4 },
                { typeof(uint), 4 },
                { typeof(long), 8 },
                { typeof(ulong), 8 },
                { typeof(float), 4 },
                { typeof(double), 8 },
            };

        private static readonly Dictionary<string, string> operatorSymbols = new Dictionary<string, string>
        {
            { "op_Addition", "+" },
            { "op_Subtraction", "-" },
            { "op_Multiply", "*" },
            { "op_Division", "/" },
            { "op_Append", "+" },
            { "op_Concatenate", "+" },
            { "op_Modulus", "%" },
            { "op_BitwiseAnd", "&" },
            { "op_BitwiseOr", "|" },
            { "op_ExclusiveOr", "^" },
            { "op_LeftShift", "<<" },
            { "op_LogicalNot", "!" },
            { "op_RightShift", ">>" },
            { "op_UnaryPlus", "+" },
            { "op_UnaryNegation", "-" },
            { "op_Equality", "==" },
            { "op_Inequality", "!=" },
            { "op_LessThanOrEqual", "<=" },
            { "op_GreaterThanOrEqual", ">=" },
            { "op_LessThan", "<" },
            { "op_GreaterThan", ">" },
            { "op_PipeRight", "|>" },
            { "op_PipeLeft", "<|" },
            { "op_Dereference", "!" },
            { "op_ComposeRight", ">>" },
            { "op_ComposeLeft", "<<" },
            { "op_AdditionAssignment", "+=" },
            { "op_SubtractionAssignment", "-=" },
            { "op_MultiplyAssignment", "*=" },
            { "op_DivisionAssignment", "/=" },
        };

        public static readonly Type[] EmptyTypes =
#if NET35 || NETSTANDARD1_0
            new Type[0];
#else
            Type.EmptyTypes;
#endif

        public static int SizeOf(this Type type) =>
#if NETSTANDARD1_0
            sizeOf.TryGetValue(type, out var size) ?
                size :
                type.IsValueType() ?
                    Buffer.ByteLength(Array.CreateInstance(type, 1)) :
                    -1;
#else
            sizeOf.TryGetValue(type, out var size) ?
                size :
                type.IsValueType() ?
                    Marshal.SizeOf(type) :
                    -1;
#endif

        public static bool IsClsCompliant(this Type type) =>
            knownClsCompilant.Contains(type);

        public static bool IsInteger(this Type type) =>
            knownInteger.Contains(type);

        public static bool IsTypeConstructor(Type type) =>
            type.IsGenericTypeDefinition() && (type.GetGenericArguments().Length == 1);

        public static string GetName(this MemberInfo member, NameOptions option = NameOptions.WithGenericParameters)
        {
            var type = member.AsType();
            if (type is Type ? type.IsGenericParameter : false)
            {
                return type!.Name;
            }

            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            if (member is MethodInfo method1 && method1.IsSpecialName)
            {
                if (option == NameOptions.Symbols &&
                    operatorSymbols.TryGetValue(name, out var symbol))
                {
                    name = symbol;
                }
                else if (method1.DeclaringType is Type type2 &&
                    type2.GetDeclaredProperties().FirstOrDefault(p =>
                        method1.Equals(p.GetGetMethod()) || method1.Equals(p.GetSetMethod())) is PropertyInfo property)
                {
                    name = property.Name;
                }

                // TODO: event
            }

            switch (type, member)
            {
                case (Type _, _) when (option == NameOptions.WithGenericParameters) && type!.IsGenericType():
                    var gta = StringUtilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetName(ga, option)));
                    return $"{name}<{gta}>";

                case (_, MethodInfo method2) when (option == NameOptions.WithGenericParameters) && method2.IsGenericMethod:
                    var gma = StringUtilities.Join(
                        ",",
                        method2.GetGenericArguments().Select(ga => GetName(ga, option)));
                    return $"{name}<{gma}>";

                default:
                    return $"{name}";
            }
        }

        public static string GetName(this Type type, NameOptions option = NameOptions.WithGenericParameters) =>
            type.AsMemberInfo().GetName(option);

        private static string Append(this string a, string b) =>
            a + b;

        public static string GetFullName(this MemberInfo member, bool withGenericParameters = true)
        {
            var type = member.AsType();
            if (type is Type ? type.IsGenericParameter : false)
            {
                return type!.Name;
            }

            var parentNames = member.DeclaringType?.GetFullName(withGenericParameters) ??
                type?.Namespace ??
                string.Empty;
            var dot = parentNames.Length >= 1 ? "." : string.Empty;
            var name = member.Name.IndexOf('`') switch
            {
                -1 => member.Name,
                int index => member.Name.Substring(0, index)
            };

            switch (type, member)
            {
                case (Type _, _) when withGenericParameters && type!.IsGenericType():
                    var gta = StringUtilities.Join(
                        ",",
                        type!.GetGenericArguments().Select(ga => GetFullName(ga, withGenericParameters)));
                    return $"{parentNames}{dot}{name}<{gta}>";

                case (_, ConstructorInfo constructor) when withGenericParameters && constructor.DeclaringType.IsGenericType():
                    var gca = StringUtilities.Join(
                        ",",
                        constructor.DeclaringType.GetGenericArguments().Select(ga => GetFullName(ga, withGenericParameters)));
                    return $"{parentNames}<{gca}>";

                case (_, ConstructorInfo _):
                    return $"{parentNames}";

                case (_, MethodInfo method) when withGenericParameters && method.IsGenericMethod:
                    var gma = StringUtilities.Join(
                        ",",
                        method.GetGenericArguments().Select(ga => GetFullName(ga, withGenericParameters)));
                    return $"{parentNames}{dot}{name}<{gma}>";

                default:
                    return $"{parentNames}{dot}{name}";
            }
        }

        public static string GetFullName(this Type type, bool withGenericParameters = true) =>
            type.AsMemberInfo().GetFullName(withGenericParameters);

        public static bool IsConvertibleFrom(this Type type, Type from)
        {
            if (type.Equals(from))
            {
                return true;
            }

            if (type.IsPrimitive() && from.IsPrimitive())
            {
                return primitiveConvertibles.Contains((from, type));
            }
            else
            {
                return type.IsAssignableFrom(from);
            }
        }

        public static (Type, Type[]) GetDelegateSignature(Type delegateType)
        {
            Debug.Assert(typeof(Delegate).IsConvertibleFrom(delegateType));

            var invoke = delegateType.GetDeclaredMethod("Invoke");

            var parameters = invoke.GetParameters();
            return parameters.Length >= 1 ?
                (invoke.ReturnType, parameters.Select(parameter => parameter.ParameterType).Memoize()) :
                (invoke.ReturnType, EmptyTypes);
        }

        public static int Compare(Type tx, Type ty)
        {
            if (tx.Equals(ty))
            {
                return 0;
            }

            if (ty.IsConvertibleFrom(tx))
            {
                return 1;
            }
            if (tx.IsConvertibleFrom(ty))
            {
                return -1;
            }

            if (tx.IsInteger() && ty.IsInteger())
            {
                var txs = tx.SizeOf();
                var tys = ty.SizeOf();
                if (txs < tys)
                {
                    return -1;
                }
                if (txs > tys)
                {
                    return 1;
                }

                var txct = tx.IsClsCompliant();
                var tyct = ty.IsClsCompliant();
                if (txct && !tyct)
                {
                    return -1;
                }
                if (!txct && tyct)
                {
                    return 1;
                }
            }

            if (tx.IsInteger())
            {
                return -1;
            }
            if (ty.IsInteger())
            {
                return -1;
            }

            if (tx.IsPrimitive())
            {
                return -1;
            }
            if (ty.IsPrimitive())
            {
                return -1;
            }

            if (tx.IsValueType() && ty.IsValueType())
            {
                var txs = tx.SizeOf();
                var tys = ty.SizeOf();
                if (txs < tys)
                {
                    return -1;
                }
                if (txs > tys)
                {
                    return 1;
                }
            }

            if (tx.IsGenericType() && ty.IsGenericType())
            {
                if (!tx.IsGenericParameter && ty.IsGenericParameter)
                {
                    return -1;
                }
                if (tx.IsGenericParameter && !ty.IsGenericParameter)
                {
                    return 1;
                }
                if (!tx.IsGenericParameter && !ty.IsGenericParameter)
                {
                    var txgps = tx.GetGenericArguments();
                    var tygps = ty.GetGenericArguments();
                    if (txgps.Length < tygps.Length)
                    {
                        return -1;
                    }
                    if (txgps.Length > tygps.Length)
                    {
                        return 1;
                    }

                    if (txgps.
                        Zip(tygps, (x, y) => Compare(x, y)).
                        FirstOrDefault(r => r != 0) is int r2 && r2 != 0)
                    {
                        return r2;
                    }
                }
            }

            // TODO: array

            if (tx.DeclaringType is Type txt &&
                ty.DeclaringType is Type tyt &&
                Compare(txt, tyt) is int r && r != 0)
            {
                return r;
            }

            // Not compatible types.
            return tx.FullName.CompareTo(ty.FullName);
        }

        public static int Compare(MethodBase mx, MethodBase my)
        {
            if (mx.Equals(my))
            {
                return 0;
            }

            if (mx.DeclaringType is Type mxt &&
                my.DeclaringType is Type myt &&
                Compare(mxt, myt) is int r && r != 0)
            {
                return r;
            }

            if (!mx.IsGenericMethod && my.IsGenericMethod)
            {
                return -1;
            }
            if (mx.IsGenericMethod && !my.IsGenericMethod)
            {
                return 1;
            }
            if (!mx.IsGenericMethod && !my.IsGenericMethod)
            {
                var txgps = mx.GetGenericArguments();
                var tygps = my.GetGenericArguments();
                if (txgps.Length < tygps.Length)
                {
                    return -1;
                }
                if (txgps.Length > tygps.Length)
                {
                    return 1;
                }

                if (txgps.
                    Zip(tygps, (x, y) => Compare(x, y)).
                    FirstOrDefault(r => r != 0) is int r4 && r4 != 0)
                {
                    return r4;
                }
            }

            var mxps = mx.GetParameters().Select(p => p.ParameterType).Memoize();
            var myps = my.GetParameters().Select(p => p.ParameterType).Memoize();

            if (mxps.Length < myps.Length)
            {
                return -1;
            }
            if (mxps.Length > myps.Length)
            {
                return 1;
            }

            if (mxps.
                Zip(myps, (x, y) => Compare(x, y)).
                FirstOrDefault(r => r != 0) is int r2 && r2 != 0)
            {
                return r2;
            }

            var mxrt = mx is MethodInfo mxi ? mxi.ReturnType : mx.DeclaringType;
            var myrt = my is MethodInfo myi ? myi.ReturnType : my.DeclaringType;
            if (Compare(mxrt, myrt) is int r3 && r3 != 0)
            {
                return r3;
            }

            // Not compatible methods.
            return mx.Name.CompareTo(my.Name);
        }
    }
}
