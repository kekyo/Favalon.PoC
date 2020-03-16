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

using System;

namespace Favalet.Expressions
{
    public sealed class CLRExpressionFactory :
        ExpressionFactory, IExpressionFactory
    {
        private CLRExpressionFactory()
        { }

        IExpression IExpressionFactory.Numeric(string value) =>
            FromNumeric(value);

        IExpression IExpressionFactory.String(string value) =>
            ConstantTerm.Create(value);

        public static new readonly IExpressionFactory Instance =
            new CLRExpressionFactory();

        public static ConstantTerm Constant(object value) =>
            ConstantTerm.Create(value);

        public static ConstantTerm FromNumeric(string value)
        {
            if (byte.TryParse(value, out var byteValue))
            {
                return ConstantTerm.Create(byteValue);
            }
            else if (short.TryParse(value, out var shortValue))
            {
                return ConstantTerm.Create(shortValue);
            }
            else if (int.TryParse(value, out var intValue))
            {
                return ConstantTerm.Create(intValue);
            }
            else if (long.TryParse(value, out var longValue))
            {
                return ConstantTerm.Create(longValue);
            }
            else if (float.TryParse(value, out var floatValue))
            {
                return ConstantTerm.Create(floatValue);
            }
            else if (double.TryParse(value, out var doubleValue))
            {
                return ConstantTerm.Create(doubleValue);
            }
            else
            {
                throw new FormatException(value);
            }
        }
    }
}
