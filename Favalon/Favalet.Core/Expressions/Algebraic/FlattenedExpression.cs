using Favalet.Contexts;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    internal abstract class FlattenedExpression :
        Expression, IExpression
    {
        public readonly IExpression[] Operands;

        protected FlattenedExpression(IExpression[] operands) =>
            this.Operands = operands;

        public IExpression HigherOrder =>
            throw new InvalidOperationException();

        public override sealed int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, operand) => agg ^ operand.GetHashCode());

        public abstract bool Equals(IExpression? other);

        public IExpression Reduce(IReduceContext context) =>
            throw new InvalidOperationException();

        public void Deconstruct(out IExpression[] operands) =>
            operands = this.Operands;

        public static IEnumerable<IExpression> Flatten<TBinaryExpression>(
            IExpression left, IExpression right)
            where TBinaryExpression : IBinaryExpression
        {
            var lf = left is TBinaryExpression lb ?
                Flatten<TBinaryExpression>(lb.Left, lb.Right) :
                new[] { left };

            var rf = right is TBinaryExpression rb ?
                Flatten<TBinaryExpression>(rb.Left, rb.Right) :
                new[] { right };

            return lf.Concat(rf);
        }

        public static IExpression Flatten(IExpression expression) =>
            expression switch
            {
                IAndExpression and => new AndFlattenedExpression(
                    Flatten<IAndExpression>(and.Left, and.Right).Memoize()),
                IOrExpression or => new OrFlattenedExpression(
                    Flatten<IOrExpression>(or.Left, or.Right).Memoize()),
                _ => expression
            };

        public static IEnumerable<IExpression> FlattenAll<TBinaryExpression>(
            IExpression left, IExpression right)
            where TBinaryExpression : IBinaryExpression
        {
            var lf = left is TBinaryExpression lb ?
                FlattenAll<TBinaryExpression>(lb.Left, lb.Right) :
                new[] { FlattenAll(left) };

            var rf = right is TBinaryExpression rb ?
                FlattenAll<TBinaryExpression>(rb.Left, rb.Right) :
                new[] { FlattenAll(right) };

            return lf.Concat(rf);
        }

        public static IExpression FlattenAll(IExpression expression) =>
            expression switch
            {
                IAndExpression and => new AndFlattenedExpression(
                    FlattenAll<IAndExpression>(and.Left, and.Right).Memoize()),
                IOrExpression or => new OrFlattenedExpression(
                    FlattenAll<IOrExpression>(or.Left, or.Right).Memoize()),
                _ => expression
            };
    }

    internal sealed class AndFlattenedExpression : FlattenedExpression
    {
        public AndFlattenedExpression(IExpression[] operands) :
            base(operands)
        { }

        public override bool Equals(IExpression? other) =>
            other is AndFlattenedExpression rhs &&
            this.Operands.EqualsPartiallyOrdered(rhs.Operands);

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    "(" + string.Join(" && ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")",
                _ =>
                    "(AndFlattened " + string.Join(" ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")"
            };
    }

    internal sealed class OrFlattenedExpression : FlattenedExpression
    {
        public OrFlattenedExpression(IExpression[] operands) :
            base(operands)
        { }

        public override bool Equals(IExpression? other) =>
            other is OrFlattenedExpression rhs &&
            this.Operands.EqualsPartiallyOrdered(rhs.Operands);

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    "(" + string.Join(" || ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")",
                _ =>
                    "(OrFlattened " + string.Join(" ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")"
            };
    }
}
