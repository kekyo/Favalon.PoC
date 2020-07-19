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

        protected virtual IEnumerable<IExpression> ReduceForOr(
            IEnumerable<IExpression> expressions) =>
            expressions.Distinct();   // Idempotence / Commutative / Associative

        protected virtual IEnumerable<IExpression> ReduceForAnd(
            IEnumerable<IExpression> expressions) =>
            expressions.Distinct();   // Idempotence / Commutative / Associative

        private IExpression CombineIfRequired(
            IEnumerable<IExpression> expressions,
            Func<IEnumerable<IExpression>, IEnumerable<IExpression>> reducer,
            Func<IExpression[], IExpression> creator)
        {
            var exprs = reducer(
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
                        ReduceForOr,
                        OrExpression.Create),
                IAndBinaryExpression and =>
                    this.CombineIfRequired(
                        EnumerateByBinaryType(and),
                        ReduceForAnd,
                        AndExpression.Create),

                // TODO: IOrExpression and IAndExpression
                _ => operand
            };

        protected enum ReduceResults
        {
            NonRelated,
            AcceptLeft,
            AcceptRight,
        }

        protected virtual ReduceResults ChoiceForAnd(
            IExpression left, IExpression right) =>
            left.Equals(right) ? ReduceResults.AcceptRight : ReduceResults.NonRelated;

        protected virtual ReduceResults ChoiceForOr(
            IExpression left, IExpression right) =>
            left.Equals(right) ? ReduceResults.AcceptLeft : ReduceResults.NonRelated;

        private IExpression ReduceAbsorption(IExpression operand)
        {
            // Absorption
            if (operand is ISetExpression set)
            {
                Debug.Assert(set.Operands.Length >= 2);

                var reducedOpers = set.Operands.
                    Select(oper => this.ReduceAbsorption(oper)).
                    Memoize();

                if (set is IAndExpression and)
                {
                    // Distribute
                    var r = reducedOpers.Aggregate((left, right) =>
                    {
                        var leftOr = left as IOrExpression;
                        var rightOr = right as IOrExpression;

                        if ((leftOr != null) && (rightOr != null))
                        {
                            return rightOr.Operands.Aggregate((rightOrLeft, rightOrRight) =>
                            {
                                return leftOr.Operands.Aggregate((leftOrLeft, leftOrRight) =>
                                {
                                    var aLeft = AndExpression.Create(leftOrLeft, rightOrLeft);
                                    var aRight = AndExpression.Create(leftOrRight, rightOrRight);
                                    return OrExpression.Create(aLeft, aRight);
                                });
                            });
                        }

                        if (rightOr != null)
                        {
                            return rightOr.Operands.Aggregate((orLeft, orRight) =>
                            {
                                var aLeft = AndExpression.Create(left, orLeft);
                                var aRight = AndExpression.Create(left, orRight);
                                return OrExpression.Create(aLeft, aRight);
                            });
                        }
                        
                        if (leftOr != null)
                        {
                            return leftOr.Operands.Aggregate((orLeft, orRight) =>
                            {
                                var aLeft = AndExpression.Create(orLeft, right);
                                var aRight = AndExpression.Create(orRight, right);
                                return OrExpression.Create(aLeft, aRight);
                            });
                        }
                    });
                }

                if (set is IOrExpression or)
                {
                    if (reducedOpers.Cast<IExpression?>().Aggregate(
                        (agg, v) =>
                            (agg is IExpression a &&
                            v is IAndExpression(IExpression[] subopers) &&
                            subopers.Any(suboper =>
                                this.ChoiceForOr(suboper, a) != ReduceResults.NonRelated)) ?
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
            return this.ReduceAbsorption(combined);
        }
    }
}
