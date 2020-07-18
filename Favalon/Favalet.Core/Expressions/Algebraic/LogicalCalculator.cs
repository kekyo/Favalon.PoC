using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalCalculator
    {
        IExpression Compute(IExpression operand);
    }

    public class LogicalCalculator :
        ILogicalCalculator
    {
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

        protected virtual IEnumerable<IExpression> MakeSimpleOr(
            IEnumerable<IExpression> expressions) =>
            expressions.Distinct();   // Idempotence / Commutative / Associative

        protected virtual IEnumerable<IExpression> MakeSimpleAnd(
            IEnumerable<IExpression> expressions) =>
            expressions.Distinct();   // Idempotence / Commutative / Associative

        private IExpression CombineIfRequired(
            IEnumerable<IExpression> expressions,
            Func<IEnumerable<IExpression>, IEnumerable<IExpression>> toSimplify,
            Func<IExpression[], IExpression> creator)
        {
            var exprs = toSimplify(
                expressions.Select(oper => this.CombineByBinaryType(oper))).
                Memoize();
            Debug.Assert(exprs.Length >= 1);

            return (exprs.Length == 1) ? exprs[0] : creator(exprs);
        }

        private IExpression CombineByBinaryType(
            IExpression operand) =>
            operand switch
            {
                IOrBinaryExpression or =>
                    this.CombineIfRequired(
                        EnumerateByBinaryType(or),
                        MakeSimpleOr,
                        OrExpression.Create),
                IAndBinaryExpression and =>
                    this.CombineIfRequired(
                        EnumerateByBinaryType(and),
                        MakeSimpleAnd,
                        AndExpression.Create),

                // TODO: IOrExpression and IAndExpression
                _ => operand
            };

        private IExpression ComputeAbsorption(IExpression operand)
        {
            // Absorption
            if (operand is ISetExpression set)
            {
                Debug.Assert(set.Operands.Length >= 2);

                var reducedOpers = set.Operands.
                    Select(oper => this.ComputeAbsorption(oper)).
                    Memoize();

                if (set is IAndExpression and)
                {
                    if (reducedOpers.Cast<IExpression?>().Aggregate(
                        (agg, v) =>
                            (agg is IExpression a &&
                            v is IOrExpression(IExpression[] subopers) &&
                            subopers.Any(suboper => suboper.Equals(a))) ?
                                agg : null) is IExpression oper2)
                    {
                        return oper2;
                    }
                    else if (and.Operands.EqualsPartiallyOrdered(reducedOpers))
                    {
                        return operand;
                    }
                    else
                    {
                        return AndExpression.Create(reducedOpers);
                    }
                }

                if (set is IOrExpression or)
                {
                    if (reducedOpers.Cast<IExpression?>().Aggregate(
                        (agg, v) =>
                            (agg is IExpression a &&
                            v is IAndExpression(IExpression[] subopers) &&
                            subopers.Any(suboper => suboper.Equals(a))) ?
                                agg : null) is IExpression oper2)
                    {
                        return oper2;
                    }
                    else if (or.Operands.EqualsPartiallyOrdered(reducedOpers))
                    {
                        return operand;
                    }
                    else
                    {
                        return OrExpression.Create(reducedOpers);
                    }
                }

                throw new InvalidOperationException();
            }

            return operand;
        }

        public IExpression Compute(IExpression operand)
        {
            var combined = this.CombineByBinaryType(operand);
            return this.ComputeAbsorption(combined);
        }
    }
}
