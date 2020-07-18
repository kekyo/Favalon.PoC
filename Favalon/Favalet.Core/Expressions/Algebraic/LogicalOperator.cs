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
            IReduceContext context,
            IEnumerable<IExpression> expressions,
            Func<IExpression[], IExpression> creator)
        {
            var exprs = expressions.
                Select(oper => CombineByBinaryType(context, oper)).
                Distinct().   // Commutative
                Select(oper => oper.Reduce(context)).
                ToArray();
            Debug.Assert(exprs.Length >= 1);

            return (exprs.Length == 1) ? exprs[0] : creator(exprs);
        }

        private static IExpression CombineByBinaryType(
            IReduceContext context,
            IExpression operand) =>
            operand switch
            {
                IOrBinaryExpression or =>
                    CombineIfRequired(
                        context,
                        EnumerateByBinaryType(or),
                        OrExpression.Create),
                IAndBinaryExpression and =>
                    CombineIfRequired(
                        context,
                        EnumerateByBinaryType(and),
                        AndExpression.Create),
                _ => operand
            };

        private static IExpression Reduce(
            IReduceContext context,
            IExpression operand)
        {
            // Absorption
            if (operand is IAndExpression and)
            {
                Debug.Assert(and.Operands.Length >= 2);

                var opers = and.Operands.
                    Select(oper => Reduce(context, oper)).
                    ToArray();

                if (opers.Cast<IExpression?>().Aggregate(
                    (agg, v) =>
                        (agg is IExpression a &&
                        v is IOrExpression(IExpression[] subopers) &&
                        subopers.Any(suboper => suboper.Equals(a))) ?
                            agg : null) is IExpression oper2)
                {
                    return oper2;
                }
                else
                {
                    return operand;
                }
            }

            if (operand is IOrExpression or)
            {
                Debug.Assert(or.Operands.Length >= 2);

                var opers = or.Operands.
                    Select(oper => Reduce(context, oper)).
                    ToArray();

                if (opers.Cast<IExpression?>().Aggregate(
                    (agg, v) =>
                        (agg is IExpression a &&
                        v is IAndExpression(IExpression[] subopers) &&
                        subopers.Any(suboper => suboper.Equals(a))) ?
                            agg : null) is IExpression oper2)
                {
                    return oper2;
                }
                else
                {
                    return operand;
                }
            }
            else
            {
                return operand;
            }
        }

        public IExpression Reduce(IReduceContext context) =>
            Reduce(context, CombineByBinaryType(context, this.Operand));

        public static LogicalOperator Create(IBinaryExpression operand) =>
            new LogicalOperator(operand);
    }
}
