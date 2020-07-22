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

        public IExpression Reduce(IReduceContext context) =>
            calculator.Compute(this.Operand.Reduce(context));

        public override string GetPrettyString(PrettyStringTypes type) =>
            $"(Logical {this.Operand.GetPrettyString(type)})";

        public static LogicalExpression Create(
            IExpression operand, IExpression higherOrder) =>
            new LogicalExpression(operand, higherOrder);
        public static LogicalExpression Create(
            IExpression operand) =>
            new LogicalExpression(operand, UnspecifiedTerm.Instance);
    }
}
