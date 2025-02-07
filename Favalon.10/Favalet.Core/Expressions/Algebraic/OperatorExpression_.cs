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

using Favalet.Contexts;
using Favalet.Expressions.Comparer;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Algebraic
{
    public interface IOperatorExpression_ :
        IExpression
    {
        IExpression[] Operands { get; }
    }

    public abstract class OperatorExpression_<TOperator> :
        Expression, IOperatorExpression_, IInferrableExpression, IReducibleExpression, IComparable<IExpression>
        where TOperator : class, IOperatorExpression_
    {
        public readonly IExpression[] Operands;

        protected OperatorExpression_(IExpression[] expressions, IExpression higherOrder)
        {
            this.Operands = expressions;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression[] IOperatorExpression_.Operands =>
            this.Operands;

        protected static IExpression? From(
            IEnumerable<IExpression> operands,
            Func<IExpression[], TOperator> creator,
            bool isStrict)
        {
            // It digs only first depth.
            var ops = isStrict ?
                operands.
                    SelectMany(operand => operand is TOperator(IExpression[] operands) ?
                        operands : new[] { operand }).
                    Memoize() :
                operands.
                    SelectMany(operand => operand is TOperator(IExpression[] operands) ?
                        operands : new[] { operand }).
                    Distinct(ExactEqualityComparer.Instance).
                    Memoize();

            return ops.Length switch
            {
                0 => null,
                1 when !isStrict => ops[0],
                _ => creator(ops)
            };
        }

        protected abstract IExpression? From(
            IEnumerable<IExpression> operands, IExpression higherOrder);

        public IExpression Infer(IInferContext context)
        {
            var higherOrder = this.HigherOrder.InferIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.InferIfRequired(context)).
                Distinct(LogicalEqualityComparer.Instance).
                Memoize();

            var operandHigherOrders = this.From(
                operands.Select(operand => operand.HigherOrder),
                UnspecifiedTerm.Instance)!;

            context.Unify(higherOrder, operandHigherOrders);

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, higherOrder)!;
            }
        }

        public virtual IExpression Fixup(IFixupContext context)
        {
            var higherOrder = this.HigherOrder.FixupIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.FixupIfRequired(context)).
                Distinct(LogicalEqualityComparer.Instance).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, higherOrder)!;
            }
        }

        public virtual IExpression Reduce(IReduceContext context)
        {
            var higherOrder = this.HigherOrder.ReduceIfRequired(context);
            var operands = this.Operands.
                Select(operand => operand.ReduceIfRequired(context)).
                Distinct(LogicalEqualityComparer.Instance).
                Memoize();

            if (this.HigherOrder.ExactEquals(higherOrder) &&
                this.Operands.ExactSequenceEqual(operands))
            {
                return this;
            }
            else
            {
                return From(operands, higherOrder)!;
            }
        }

        public override bool Equals(IExpression? rhs, IEqualityComparer<IExpression> comparer) =>
            rhs is TOperator oper &&
                this.Operands.SequenceEqual(oper.Operands, comparer);

        public override sealed int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        int IComparable<IExpression>.CompareTo(IExpression rhs)
        {
            if (rhs is TOperator op)
            {
                return this.Operands.
                    Zip(op.Operands, ExpressionComparer.Compare).
                    FirstOrDefault(r => r != 0);
            }
            return this.GetHashCode().CompareTo(rhs.GetHashCode());
        }

        public override sealed T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.SuppressHigherOrder, this.Operands);
    }

    public static class OperatorExpressionExtension
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Deconstruct(this IOperatorExpression_ oper, out IExpression[] operands) =>
            operands = oper.Operands;
    }
}
