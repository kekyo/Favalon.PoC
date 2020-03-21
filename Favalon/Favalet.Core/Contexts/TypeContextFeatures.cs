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

using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;

namespace Favalet.Contexts
{
    public interface ITypeContextFeatures
    {
        IExpression Identity(string identity);
        IExpression Numeric(string value);
        IExpression String(string value);
        IExpression Apply(IExpression function, IExpression argument);

        IExpression? Widen(IExpression to, IExpression from);
    }

    public class TypeContextFeatures : ITypeContextFeatures
    {
        protected TypeContextFeatures()
        { }

        public virtual IExpression Numeric(string value) =>
            throw new NotImplementedException();

        public virtual IExpression String(string value) =>
            throw new NotImplementedException();

        public virtual IExpression Identity(string identity) =>
            IdentityTerm.Create(identity, UnspecifiedTerm.Instance);

        public virtual IExpression Apply(IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument, UnspecifiedTerm.Instance);

        public virtual IExpression? Widen(IExpression to, IExpression from)
        {
            switch (to, from)
            {
                // int->object: int->object <-- object->int
                case (IFunctionDeclaredExpression(IExpression toParameter, IExpression toBody),
                      IFunctionDeclaredExpression(IExpression fromParameter, IExpression fromBody)):
                    var parameterTerm = this.Widen(fromParameter, toParameter) is IExpression ? toParameter : null;
                    var bodyTerm = this.Widen(toBody, fromBody);
                    return parameterTerm is IExpression pt && bodyTerm is IExpression bt ?
                        FunctionDeclaredExpression.From(pt, bt) :
                        null;

                // _[1]: _[1] <-- _[2]
                //case (PlaceholderTerm placeholder, PlaceholderTerm _):
                //    return placeholder;

                // _: _ <-- int
                // _: _ <-- (int + double)
                case (PlaceholderTerm placeholder, _):
                    return placeholder;

                default:
                    if (AlgebraicCalculator.Widen(to, from) is IExpression result)
                    {
                        return result;
                    }
                    // null: int <-- _   [TODO: maybe?]
                    //else if (from is PlaceholderTerm placeholder)
                    //{
                    //    return null;
                    //}
                    else
                    {
                        return null;
                    }
            }
        }

        public static readonly TypeContextFeatures Instance =
            new TypeContextFeatures();
    }
}
