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
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface ITypeContextFeatures :
        IAlgebraicCalculator<IInferContext?>
    {
        IExpression CreateIdentity(string identity);
        IExpression CreateNumeric(string value);
        IExpression CreateString(string value);
        IExpression CreateApply(IExpression function, IExpression argument);
    }

    public class TypeContextFeatures :
        AlgebraicCalculator<IInferContext?>, ITypeContextFeatures
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

        protected override sealed IExpression? OrFrom(IEnumerable<IExpression> operands) =>
            OverloadTerm.From(operands);

        protected override IExpression? WidenCore(
            IExpression to,
            IExpression from,
            IInferContext? context)
        {
            // int->object: int->object <-- object->int
            if (to is IFunctionDeclaredExpression(IExpression tp, IExpression tr) &&
                from is IFunctionDeclaredExpression(IExpression fp, IExpression fr))
            {
                var pr = this.Widen(fp, tp, context);
                var rr = this.Widen(tr, fr, context);

                if (pr != null && rr != null)
                {
                    return FunctionDeclaredExpression.From(pr, rr);
                }
                else
                {
                    return null;
                }
            }

            if (base.WidenCore(to, from, context) is IExpression result)
            {
                return result;
            }

            if (context is IInternalInferContext c)
            {
                if (to is PlaceholderTerm tph)
                {
                    return c.Substitute(tph, from, true);
                }
                if (from is PlaceholderTerm fph)
                {
                    return c.Substitute(fph, to, false);
                }
            }

            return null;
        }

        public static new readonly TypeContextFeatures Instance =
            new TypeContextFeatures();
    }
}
