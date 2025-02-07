﻿////////////////////////////////////////////////////////////////////////////
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

using System;

namespace Favalet.Expressions
{
    public static class CLRExpressionFactory
    {
        public static ITerm Type(Type type) =>
            TypeTerm.From(type);
        public static ITerm Type<T>() =>
            TypeTerm.From(typeof(T));

        public static MethodTerm Constructor(Type type, params Type[] parameters) =>
            MethodTerm.From(type, parameters);
        public static MethodTerm Constructor<T>(params Type[] parameters) =>
            MethodTerm.From(typeof(T), parameters);
        public static MethodTerm Method(Type type, string name, params Type[] parameters) =>
            MethodTerm.From(type, name, parameters);
        public static MethodTerm Method<T>(string name, params Type[] parameters) =>
            MethodTerm.From(typeof(T), name, parameters);

        public static IExpression Constant(object value) =>
            ConstantTerm.From(value);

        public static ITerm FromNumeric(string value)
        {
            // TODO: Make numeric value expression (polymorphic)
#if true
            if (int.TryParse(value, out var intValue))
            {
                return ConstantTerm.From(intValue);
            }
            else if (long.TryParse(value, out var longValue))
            {
                return ConstantTerm.From(longValue);
            }
            else if (double.TryParse(value, out var doubleValue))
            {
                return ConstantTerm.From(doubleValue);
            }
#else
            if (byte.TryParse(value, out var byteValue))
            {
                return ConstantTerm.From(byteValue);
            }
            else if (short.TryParse(value, out var shortValue))
            {
                return ConstantTerm.From(shortValue);
            }
            else if (int.TryParse(value, out var intValue))
            {
                return ConstantTerm.From(intValue);
            }
            else if (long.TryParse(value, out var longValue))
            {
                return ConstantTerm.From(longValue);
            }
            else if (float.TryParse(value, out var floatValue))
            {
                return ConstantTerm.From(floatValue);
            }
            else if (double.TryParse(value, out var doubleValue))
            {
                return ConstantTerm.From(doubleValue);
            }
#endif
            else
            {
                throw new FormatException(value);
            }
        }
    }
}
