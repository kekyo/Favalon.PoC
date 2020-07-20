using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalCalculator
    {
        bool Equals(IExpression lhs, IExpression rhs);

        IExpression Compute(IExpression operand);
    }

    public class LogicalCalculator :
        ILogicalCalculator
    {
        private static IEnumerable<IExpression> Flatten<TBinaryExpression>(
            IExpression left, IExpression right)
            where TBinaryExpression : IBinaryExpression
        {
            var lf = left is TBinaryExpression lb ?
                Flatten<TBinaryExpression>(lb.Left, lb.Right) :
                new[] { Flatten(left) };

            var rf = right is TBinaryExpression rb ?
                Flatten<TBinaryExpression>(rb.Left, rb.Right) :
                new[] { Flatten(right) };

            return lf.Concat(rf);
        }

        private static IExpression Flatten(IExpression expression) =>
            expression switch
            {
                IAndBinaryExpression and => new AndFlattenedExpression(
                    Flatten<IAndBinaryExpression>(and.Left, and.Right).Memoize()),
                IOrBinaryExpression or => new OrFlattenedExpression(
                    Flatten<IOrBinaryExpression>(or.Left, or.Right).Memoize()),
                _ => expression
            };

        public bool Equals(IExpression lhs, IExpression rhs)
        {
            var left = Flatten(lhs);
            var right = Flatten(rhs);

            return left.Equals(right);
        }

        protected enum ReduceResults
        {
            NonRelated,
            AcceptLeft,
            AcceptRight,
        }

        private IExpression[] ComputeAbsorption<TFlattenedExpression>(
            IExpression left,
            IExpression right,
            Func<IExpression, IExpression, ReduceResults> predicate)
            where TFlattenedExpression : FlattenedExpression
        {
            var fl = Flatten(left);
            var fr = Flatten(right);

            if (fr is TFlattenedExpression(IExpression[] rightOperands))
            {
                return rightOperands.
                    SelectMany(rightOperand =>
                        predicate(fl, rightOperand) switch
                        {
                            ReduceResults.AcceptLeft => new[] { fl },
                            ReduceResults.AcceptRight => new[] { rightOperand },
                            _ => Enumerable.Empty<IExpression>()
                        }).
                    Distinct().
                    Memoize();
            }
            else if (fl is TFlattenedExpression(IExpression[] leftOperands))
            {
                return leftOperands.
                    SelectMany(leftOperand =>
                        predicate(leftOperand, fr) switch
                        {
                            ReduceResults.AcceptLeft => new[] { leftOperand },
                            ReduceResults.AcceptRight => new[] { fr },
                            _ => Enumerable.Empty<IExpression>()
                        }).
                    Distinct().
                    Memoize();
            }
            else
            {
                return ArrayEx.Empty<IExpression>();
            }
        }

        protected virtual ReduceResults ChoiceForAnd(
            IExpression left, IExpression right) =>
            // Idempotence
            this.Equals(left, right) ?
                ReduceResults.AcceptLeft :
                ReduceResults.NonRelated;

        protected virtual ReduceResults ChoiceForOr(
            IExpression left, IExpression right) =>
            // Idempotence
            this.Equals(left, right) ?
                ReduceResults.AcceptLeft :
                ReduceResults.NonRelated;

        private static IExpression? ConstructFinalResult(
            IExpression[] results,
            Func<IExpression, IExpression, IExpression> creator) =>
            results.Length switch
            {
                0 => null,
                1 => results[0],
                2 => creator(results[0], results[1]),
                _ => results.Skip(2).Aggregate(creator(results[0], results[1]), creator)
            };

        public IExpression Compute(IExpression operand)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = this.Compute(binary.Left);
                var right = this.Compute(binary.Right);

                if (binary is IAndBinaryExpression)
                {
                    // Idempotence
                    switch (this.ChoiceForAnd(left, right))
                    {
                        case ReduceResults.AcceptLeft:
                            return left;
                        case ReduceResults.AcceptRight:
                            return right;
                    }

                    // Absorption
                    var absorption = this.ComputeAbsorption<OrFlattenedExpression>(
                        left, right, this.ChoiceForAnd);
                    if (ConstructFinalResult(
                        absorption,
                        OrBinaryExpression.Create) is IExpression result2)
                    {
                        return result2;
                    }
                }
                else if (binary is IOrBinaryExpression)
                {
                    // Idempotence
                    switch (this.ChoiceForOr(left, right))
                    {
                        case ReduceResults.AcceptLeft:
                            return left;
                        case ReduceResults.AcceptRight:
                            return right;
                    }

                    // Absorption
                    var absorption = this.ComputeAbsorption<AndFlattenedExpression>(
                        left, right, this.ChoiceForOr);
                    if (ConstructFinalResult(
                        absorption,
                        AndBinaryExpression.Create) is IExpression result2)
                    {
                        return result2;
                    }
                }

                // Not changed
                if (object.ReferenceEquals(binary.Left, left) &&
                    object.ReferenceEquals(binary.Right, right))
                {
                    return binary;
                }
                else
                {
                    // Reconstruct
                    if (binary is IAndBinaryExpression)
                    {
                        return AndBinaryExpression.Create(left, right);
                    }
                    else if (binary is IOrBinaryExpression)
                    {
                        return OrBinaryExpression.Create(left, right);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return operand;
        }
    }
}
