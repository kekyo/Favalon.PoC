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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Algebraic
{
    public interface ISumExpression :
        IExpression
    {
        IExpression[] Expressions { get; }
    }

    public static class SumExpressionExtension
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Deconstruct(this ISumExpression sum, out IExpression[] expressions) =>
            expressions = sum.Expressions;
    }

    public sealed class SumExpression :
        Expression, ISumExpression, IInferrableExpression, IReducibleExpression
    {
        public readonly IExpression[] Expressions;

        private SumExpression(IExpression[] expressions, IExpression higherOrder)
        {
            this.Expressions = expressions;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression[] ISumExpression.Expressions =>
            this.Expressions;

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var expressions = this.Expressions.
                Select(expression => expression.InferIfRequired(context)).
                Memoize();

            var expressionHigherOrders = From(
                expressions.Select(expression => expression.HigherOrder),
                false)!;

            context.Unify(higherOrder, expressionHigherOrders);

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SumExpression(expressions, higherOrder);
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var expressions = this.Expressions.
                Select(expression => expression.FixupIfRequired(context)).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SumExpression(expressions, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);
            var expressions = this.Expressions.
                Select(expression => expression.ReduceIfRequired(context)).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SumExpression(expressions, higherOrder);
            }
        }

        public override bool Equals(IExpression? rhs) =>
            rhs is ISumExpression sum &&
                this.Expressions.SequenceEqual(sum.Expressions);

        public override int GetHashCode() =>
            this.Expressions.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, (object[])this.Expressions);

        public static SumExpression Create(IExpression[] expressions, IExpression higherOrder) =>
            new SumExpression(expressions, higherOrder);

        public static IExpression? From(
            IEnumerable<IExpression> expressions, bool isStrict)
        {
            // It digs only first depth.
            var ses = expressions.
                SelectMany(expression => (expression is SumExpression(IExpression[] ses)) ? ses : new[] { expression });
            var ses2 = (isStrict ? ses : ses.Distinct()).Memoize();

            return ses2.Length switch
            {
                0 => null,
                1 when !isStrict => ses2[0],
                _ => new SumExpression(ses2, UnspecifiedTerm.Instance)
            };
        }
    }
}
