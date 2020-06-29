using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IEquivalenceExpression : IExpression
    {
        IOperandExpression Operand { get; }
    }

    public sealed class EquivalenceExpression :
        IEquivalenceExpression
    {
        private readonly IOperandExpression Operand;

        private EquivalenceExpression(IOperandExpression expression) =>
            this.Operand = expression;

        IOperandExpression IEquivalenceExpression.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(IEquivalenceExpression rhs) =>
            this.Operand.Equals(rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IEquivalenceExpression rhs && Equals(rhs);

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

        public static EquivalenceExpression Create(IOperandExpression operand) =>
            new EquivalenceExpression(operand);
    }
}
