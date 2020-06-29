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
        IBinaryExpression Operand { get; }
    }

    public sealed class EquivalenceOperator :
        IEquivalenceOperator
    {
        private readonly IBinaryExpression Operand;

        private EquivalenceOperator(IBinaryExpression expression) =>
            this.Operand = expression;

        IBinaryExpression IEquivalenceOperator.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(IEquivalenceOperator rhs) =>
            this.Operand.Equals(rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IEquivalenceOperator rhs && Equals(rhs);

        private static IExpression Reduce(IReduceContext context, IExpression operand)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = Reduce(context, binary.Left);
                var right = Reduce(context, binary.Right);

                if (left.Equals(right))
                {
                    return left;
                }

                return binary switch
                {
                    IAndExpression _ => AndExpression.Create(left, right),
                    IOrExpression _ => OrExpression.Create(left, right),
                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                return operand.Reduce(context);
            }
        }

        public IExpression Reduce(IReduceContext context) =>
            Reduce(context, this.Operand);

        public static EquivalenceOperator Create(IBinaryExpression operand) =>
            new EquivalenceOperator(operand);
    }
}
