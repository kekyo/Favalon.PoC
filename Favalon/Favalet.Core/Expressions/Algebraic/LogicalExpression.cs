using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalExpression : IExpression
    {
        IExpression Operand { get; }
    }

    public sealed class LogicalExpression :
        Expression, ILogicalExpression
    {
        private static readonly LogicalCalculator calculator = new LogicalCalculator();

        public readonly IExpression Operand;

        private LogicalExpression(
            IExpression operand, IExpression higherOrder)
        {
            this.HigherOrder = higherOrder;
            this.Operand = operand;
        }

        public IExpression HigherOrder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression ILogicalExpression.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(ILogicalExpression rhs) =>
            calculator.Equals(this.Operand, rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ILogicalExpression rhs && this.Equals(rhs);

        public IExpression Infer(IReduceContext context)
        {
            var higherOrder = context.InferHigherOrder(this.HigherOrder);
            var operand = this.Operand.Infer(context);

            context.Unify(operand.HigherOrder, higherOrder);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder) &&
                object.ReferenceEquals(this.Operand, operand))
            {
                return this;
            }
            else
            {
                return new LogicalExpression(operand, higherOrder);
            }
        }

        public IExpression Fixup(IReduceContext context)
        {
            var higherOrder = context.FixupHigherOrder(this.HigherOrder);
            var operand = this.Operand.Fixup(context);

            if (object.ReferenceEquals(this.HigherOrder, higherOrder) &&
                object.ReferenceEquals(this.Operand, operand))
            {
                return this;
            }
            else
            {
                return new LogicalExpression(operand, higherOrder);
            }
        }

        public IExpression Reduce(IReduceContext context) =>
            calculator.Compute(this.Operand.Reduce(context));

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                $"Logical {this.Operand.GetPrettyString(context)}");

        public static LogicalExpression Create(
            IExpression operand, IExpression higherOrder) =>
            new LogicalExpression(operand, higherOrder);
        public static LogicalExpression Create(
            IExpression operand) =>
            new LogicalExpression(operand, UnspecifiedTerm.Instance);
    }
}
