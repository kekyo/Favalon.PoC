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
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Algebraic
{
    public interface ISumExpression :
        IExpression, IInferrableExpression, IReducibleExpression
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

    public sealed class SumExpression : Expression, ISumExpression
    {
        public readonly IExpression[] Expressions;

        private SumExpression(IExpression[] expressions, IExpression higherOrder)
        {
            this.Expressions = expressions;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        IExpression[] ISumExpression.Expressions =>
            this.Expressions;

        public IExpression Infer(IInferContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.InferIfRequired(context)).
                Memoize();
            var higherOrder = this.HigherOrder.InferIfRequired(context);

            if (From(expressions.Select(expression => expression.HigherOrder).Distinct().Memoize(), true) is IExpression ehs)
            {
                context.Unify(ehs, higherOrder);
            }

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
            var expressions = this.Expressions.
                Select(expression => expression.FixupIfRequired(context)).
                Memoize();
            var higherOrder = this.HigherOrder.FixupIfRequired(context);

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
            var expressions = this.Expressions.
                Select(expression => expression.ReduceIfRequired(context)).
                Memoize();
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);

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

        public static IExpression? From(IExpression[] expressions, bool canSuppress) =>
            expressions.Length switch
            {
                0 when canSuppress => null,
                1 when canSuppress => expressions[0],
                _ => new SumExpression(expressions, UnspecifiedTerm.Instance)
            };
    }
}
