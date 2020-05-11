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
    public interface ITypeContextFeatures : IAlgebraicCalculator
    {
        IExpression CreateIdentity(string identity);
        IExpression CreateNumeric(string value);
        IExpression CreateString(string value);
        IExpression CreateApply(IExpression function, IExpression argument);
    }

    public class TypeContextFeatures : AlgebraicCalculator, ITypeContextFeatures
    {
        protected TypeContextFeatures()
        { }

        public virtual IExpression CreateNumeric(string value) =>
            throw new NotImplementedException();

        public virtual IExpression CreateString(string value) =>
            throw new NotImplementedException();

        public virtual IExpression CreateIdentity(string identity) =>
            IdentityTerm.Create(identity, UnspecifiedTerm.Instance);

        public virtual IExpression CreateApply(IExpression function, IExpression argument) =>
            ApplyExpression.Create(function, argument, UnspecifiedTerm.Instance);

        public override IExpression? Widen(IExpression? to, IExpression? from)
        {
            switch (to, from)
            {
                // int->object: int->object <-- object->int
                case (IFunctionDeclaredExpression(IExpression toParameter, IExpression toResult),
                      IFunctionDeclaredExpression(IExpression fromParameter, IExpression fromResult)):
                    var parameter = this.Widen(fromParameter, toParameter); // is IExpression ? toParameter : null;
                    var result = this.Widen(toResult, fromResult);
                    return parameter is IExpression pr && result is IExpression rr ?
                        FunctionDeclaredExpression.From(pr, rr) :
                        null;

                // _[1]: _[1] <-- _[2]
                //case (PlaceholderTerm _, PlaceholderTerm _):
                //    return to;

                // _: _ <-- int
                // _: _ <-- (int + double)
                //case (PlaceholderTerm _, _):
                //    return to;

                default:
                    if (base.Widen(to, from) is IExpression widen)
                    {
                        return widen;
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

        public static new readonly TypeContextFeatures Instance =
            new TypeContextFeatures();
    }
}
