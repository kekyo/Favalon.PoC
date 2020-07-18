using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalOperator : IExpression
    {
        IBinaryExpression Operand { get; }
    }

    public sealed class LogicalOperator :
        ILogicalOperator
    {
        private readonly IBinaryExpression Operand;

        private LogicalOperator(IBinaryExpression expression) =>
            this.Operand = expression;

        IBinaryExpression ILogicalOperator.Operand =>
            this.Operand;

        public override int GetHashCode() =>
            this.Operand.GetHashCode();

        public bool Equals(ILogicalOperator rhs) =>
            this.Operand.Equals(rhs.Operand);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is ILogicalOperator rhs && Equals(rhs);

        private static IEnumerable<IExpression> EnumerateByBinaryType<TBinaryExpression>(
            TBinaryExpression binary)
            where TBinaryExpression : IBinaryExpression =>
            binary switch
            {
                IBinaryExpression(TBinaryExpression left, TBinaryExpression right) =>
                    EnumerateByBinaryType(left).Concat(EnumerateByBinaryType(right)),
                IBinaryExpression(TBinaryExpression left, IExpression right) =>
                    EnumerateByBinaryType(left).Concat(new[] { right }),
                IBinaryExpression(IExpression left, TBinaryExpression right) =>
                    new[] { left }.Concat(EnumerateByBinaryType(right)),
                _ =>
                    new[] { binary.Left, binary.Right }
            };

        private static IExpression CombineIfRequired(
            IEnumerable<IExpression> expressions,
            Func<IExpression[], IExpression> creator)
        {
            var exprs = expressions.Distinct().ToArray();
            Debug.Assert(exprs.Length >= 1);
            return (exprs.Length == 1) ? exprs[0] : creator(exprs);
        }

        private static IExpression CombineByBinaryType(IExpression operand) =>
            operand switch
            {
                IOrBinaryExpression or =>
                    CombineIfRequired(EnumerateByBinaryType(or), OrExpression.Create),
                IAndBinaryExpression and =>
                    CombineIfRequired(EnumerateByBinaryType(and), AndExpression.Create),
                _ => operand
            };

        private static IExpression Reduce(IReduceContext context, IExpression operand)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = CombineByBinaryType(binary.Left.Reduce(context));
                var right = CombineByBinaryType(binary.Right.Reduce(context));

                if (left is ISetExpression l &&
                    right is ISetExpression r &&
                    l.Operands.EqualsPartiallyOrdered(r.Operands))
                {
                    return left;
                }

                return (binary, left, right) switch
                {
                    // Absorption
                    (IAndBinaryExpression _, IExpression _, IOrBinaryExpression(IExpression l1, IExpression r1))
                        when left.Equals(l1) || left.Equals(r1) =>
                        left,
                    (IAndBinaryExpression _, IOrBinaryExpression(IExpression l1, IExpression r1), IExpression _)
                        when right.Equals(l1) || right.Equals(r1) =>
                        right,
                    (IOrBinaryExpression _, IExpression _, IAndBinaryExpression(IExpression l1, IExpression r1))
                        when left.Equals(l1) || left.Equals(r1) =>
                        left,
                    (IOrBinaryExpression _, IAndBinaryExpression(IExpression l1, IExpression r1), IExpression _)
                        when right.Equals(l1) || right.Equals(r1) =>
                        right,

                    // Commutative
                    (_, IAndBinaryExpression(IExpression l1, IExpression r1), IAndBinaryExpression(IExpression l2, IExpression r2))
                        when l1.Equals(r2) && r1.Equals(l2) =>
                        left,
                    (_, IOrBinaryExpression(IExpression l1, IExpression r1), IOrBinaryExpression(IExpression l2, IExpression r2))
                        when l1.Equals(r2) && r1.Equals(l2) =>
                        left,

                    // Reduced
                    (IAndBinaryExpression _, _, _) =>
                        AndBinaryExpression.Create(left, right),
                    (IOrBinaryExpression _, _, _) =>
                        OrBinaryExpression.Create(left, right),

                    _ => throw new InvalidOperationException()
                };
            }
            else
            {
                return operand;
            }
        }

        public IExpression Reduce(IReduceContext context) =>
            Reduce(context, this.Operand);

        public static LogicalOperator Create(IBinaryExpression operand) =>
            new LogicalOperator(operand);
    }
}
