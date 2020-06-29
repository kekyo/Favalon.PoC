using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IEquivalenceOperator : IExpression
    {
        IOperandExpression Operand { get; }
    }

    public sealed class EquivalenceOperator :
        IEquivalenceOperator
    {
        private readonly IOperandExpression Operand;

        private EquivalenceOperator(IOperandExpression expression) =>
            this.Operand = expression;

        IOperandExpression IEquivalenceOperator.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(IEquivalenceOperator rhs) =>
            this.Operand.Equals(rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IEquivalenceOperator rhs && Equals(rhs);

        private static IExpression Reduce(IReduceContext context, IOperandExpression operand)
        {
            var suppressed =
                operand.Operands.
                Select(expr => expr is IOperandExpression oper ?
                    Reduce(context, oper) :
                    expr.Reduce(context)).
                Distinct().
                ToArray();

            return (suppressed.Length, operand) switch
            {
                (1, _) => suppressed[0],
                (_, IAndExpression _) => AndExpression.Create(suppressed),
                (_, IOrExpression _) => OrExpression.Create(suppressed),
                _ => throw new InvalidOperationException()
            };
        }

        public IExpression Reduce(IReduceContext context) =>
            Reduce(context, this.Operand);

        public static EquivalenceOperator Create(IOperandExpression operand) =>
            new EquivalenceOperator(operand);
    }
}
