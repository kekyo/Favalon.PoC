using Favalet.Contexts;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ISetExpression :
        IExpression
    {
         IExpression[] Expressions { get; }
    }

    public sealed class SetExpression :
        Expression, ISetExpression, IInferrableExpression, IReducibleExpression
    {
        private readonly ValueLazy<SetExpression> higherOrder;

        public readonly IExpression[] Expressions;

        private SetExpression(IExpression[] expressions)
        {
            this.Expressions = expressions;
            this.higherOrder = ValueLazy.Create(() =>
                new SetExpression(this.Expressions.Select(expression => expression.HigherOrder).Memoize()));
        }

        public override IExpression HigherOrder =>
            this.higherOrder.Value;

        IExpression[] ISetExpression.Expressions =>
            this.Expressions;

        public IExpression Infer(IInferContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.InferIfRequired(context)).
                Memoize();

            if (this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SetExpression(expressions);
            }
        }

        public IExpression Fixup(IFixupContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.FixupIfRequired(context)).
                Memoize();

            if (this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SetExpression(expressions);
            }
        }

        public IExpression Reduce(IReduceContext context)
        {
            var expressions = this.Expressions.
                Select(expression => expression.ReduceIfRequired(context)).
                Memoize();

            if (this.Expressions.ExactSequenceEqual(expressions))
            {
                return this;
            }
            else
            {
                return new SetExpression(expressions);
            }
        }

        public override sealed int GetHashCode() =>
            this.Expressions.Aggregate(0, (agg, e) => agg ^ e.GetHashCode());

        public override bool Equals(
            IExpression? rhs,
            IEqualityComparer<IExpression> comparer) =>
            rhs is ISetExpression set &&
                this.Expressions.SequenceEqual(set.Expressions, comparer);

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.SuppressHigherOrder, this.Expressions);

        public static IExpression? From(params IExpression[] expressions) =>
            expressions.Length switch
            {
                0 => null,
                1 => expressions[0],
                _ => new SetExpression(expressions)
            };
    }
}
