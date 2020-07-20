using Favalet.Internal;
using System;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    internal abstract class FlattenedExpression :
        Expression, IExpression
    {
        public readonly IExpression[] Operands;

        protected FlattenedExpression(IExpression[] operands) =>
            this.Operands = operands;

        public override sealed int GetHashCode() =>
            this.Operands.Aggregate(0, (agg, operand) => agg ^ operand.GetHashCode());

        public abstract bool Equals(IExpression? other);

        public IExpression Reduce(IReduceContext context) =>
            throw new NotImplementedException();

        public void Deconstruct(out IExpression[] operands) =>
            operands = this.Operands;
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
                    "(AndF " + string.Join(" ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")"
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
                    "(OrF " + string.Join(" ", this.Operands.Select(operand => operand.GetPrettyString(type))) + ")"
            };
    }
}
