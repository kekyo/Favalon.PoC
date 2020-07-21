using Favalet.Internal;
using System;
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
        public bool Equals(IExpression lhs, IExpression rhs)
        {
            var left = FlattenedExpression.FlattenAll(lhs);
            var right = FlattenedExpression.FlattenAll(rhs);

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
            var fl = FlattenedExpression.Flatten(left);
            var fr = FlattenedExpression.Flatten(right);

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

                if (binary is IAndExpression)
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
                        OrExpression.Create) is IExpression result2)
                    {
                        return result2;
                    }
                }
                else if (binary is IOrExpression)
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
                        AndExpression.Create) is IExpression result2)
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
                    if (binary is IAndExpression)
                    {
                        return AndExpression.Create(left, right);
                    }
                    else if (binary is IOrExpression)
                    {
                        return OrExpression.Create(left, right);
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
