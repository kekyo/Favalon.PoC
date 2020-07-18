using System;
using System.Diagnostics;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalOperator : IExpression
    {
        IExpression Operand { get; }
    }

    public sealed class LogicalOperator :
        Expression, ILogicalOperator
    {
        private static readonly LogicalCalculator calculator = new LogicalCalculator();

        public readonly IExpression Operand;

        private LogicalOperator(IExpression operand) =>
            this.Operand = operand;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IExpression ILogicalOperator.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(ILogicalOperator rhs) =>
            this.Operand.Equals(rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ILogicalOperator rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context) =>
            calculator.Compute(this.Operand).
            Reduce(context);

        public override string GetPrettyString(PrettyStringTypes type) =>
            $"(Logical {this.Operand.GetPrettyString(type)})";

        public static LogicalOperator Create(IExpression operand) =>
            new LogicalOperator(operand);
    }
}
