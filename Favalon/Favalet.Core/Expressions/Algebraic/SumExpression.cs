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

using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ISumExpression :
        IExpression, IInferrableExpression, IReducibleExpression
    {
        IExpression[] Expressions { get; }
    }

    public sealed class SumExpression :
        Expression, ISumExpression
    {
        public readonly IExpression[] Expressions;
        private readonly Lazy<IExpression> higherOrder;

        private SumExpression(IExpression[] expressions)
        {
            this.Expressions = expressions;

            this.higherOrder = Lazy.Create(() =>
                From(this.Expressions.
                    Select(expression => expression.HigherOrder).
                    Memoize(),
                    false));
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        IExpression[] ISumExpression.Expressions =>
            this.Expressions;

        public IExpression Infer(IInferContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.InferIfRequired(context)).
                Memoize();

            if (this.Expressions.SequenceEqual(
                expressions, ExactEqualityComparer.Instance))
            {
                return this;
            }
            else
            {
                return new SumExpression(expressions);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.ReduceIfRequired(context)).
                Memoize();

            if (this.Expressions.SequenceEqual(
                expressions, ExactEqualityComparer.Instance))
            {
                return this;
            }
            else
            {
                return new SumExpression(expressions);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is SumExpression sum &&
                this.Expressions.SequenceEqual(sum.Expressions);

        public override int GetHashCode() =>
            this.Expressions.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, (object[])this.Expressions);

        public static IExpression From(IExpression[] expressions, bool canSuppress) =>
            expressions.Length switch
            {
                0 => TerminationTerm.Instance,
                1 when canSuppress =>
                    expressions[0],
                _ when expressions.All(expression => !(expression is TerminationTerm)) =>
                    new SumExpression(expressions),
                _ => TerminationTerm.Instance
            };
    }
}
