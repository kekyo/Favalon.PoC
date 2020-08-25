using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public interface ILogicalCalculator<TContext>
    {
        bool Equals(IExpression lhs, IExpression rhs, TContext context);
        bool ExactEquals(IExpression lhs, IExpression rhs, TContext context);

        IExpression Compute(IExpression operand, TContext context);
    }

    public class LogicalCalculator<TContext> :
        ILogicalCalculator<TContext>
    {
        public bool Equals(IExpression lhs, IExpression rhs, TContext context)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            else
            {
                var left = FlattenedExpression.FlattenAll(lhs);
                var right = FlattenedExpression.FlattenAll(rhs);

                return left.Equals(right);
            }
        }

        public bool ExactEquals(IExpression lhs, IExpression rhs, TContext context)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }
            else
            {
                var left = FlattenedExpression.FlattenAll(lhs);
                var right = FlattenedExpression.FlattenAll(rhs);

                return
                    left.Equals(right) &&
                    (left, right) switch
                    {
                        (DeadEndTerm _, DeadEndTerm _) => true,
                        (DeadEndTerm _, _) => false,
                        (_, DeadEndTerm _) => false,
                        _ => this.Equals(
                            this.Compute(lhs.HigherOrder, context),
                            this.Compute(rhs.HigherOrder, context),
                            context)
                    };
            }
        }

        protected enum ChoiceResults
        {
            NonRelated,
            Equal,
            AcceptLeft,
            AcceptRight,
        }

        protected virtual ChoiceResults ChoiceForAnd(
            IExpression left, IExpression right, TContext context) =>
            // Idempotence
            this.Equals(left, right, context) ?
                ChoiceResults.Equal :
                ChoiceResults.NonRelated;

        protected virtual ChoiceResults ChoiceForOr(
            IExpression left, IExpression right, TContext context) =>
            // Idempotence
            this.Equals(left, right, context) ?
                ChoiceResults.Equal :
                ChoiceResults.NonRelated;

        private IEnumerable<IExpression> ComputeAbsorption<TFlattenedExpression>(
            IExpression left,
            IExpression right,
            Func<IExpression, IExpression, TContext, ChoiceResults> selector,
            TContext context)
            where TFlattenedExpression : FlattenedExpression
        {
            var fl = FlattenedExpression.Flatten(left);
            var fr = FlattenedExpression.Flatten(right);

            if (fr is TFlattenedExpression(IExpression[] rightOperands))
            {
                return rightOperands.
                    SelectMany(rightOperand =>
                        selector(fl, rightOperand, context) switch
                        {
                            ChoiceResults.Equal => new[] { fl },
                            ChoiceResults.AcceptLeft => new[] { fl },
                            ChoiceResults.AcceptRight => new[] { rightOperand },
                            _ => Enumerable.Empty<IExpression>()
                        });
            }
            else if (fl is TFlattenedExpression(IExpression[] leftOperands))
            {
                return leftOperands.
                    SelectMany(leftOperand =>
                        selector(leftOperand, fr, context) switch
                        {
                            ChoiceResults.Equal => new[] { leftOperand },
                            ChoiceResults.AcceptLeft => new[] { leftOperand },
                            ChoiceResults.AcceptRight => new[] { fr },
                            _ => Enumerable.Empty<IExpression>()
                        });
            }
            else
            {
                return Enumerable.Empty<IExpression>();
            }
        }

        private IEnumerable<IExpression> ComputeShrink<TBinaryExpression>(
            IExpression left,
            IExpression right,
            Func<IExpression, IExpression, TContext, ChoiceResults> selector,
            TContext context)
            where TBinaryExpression : IBinaryExpression
        {
            var flattened = FlattenedExpression.Flatten<TBinaryExpression>(left, right);
            var candidates = new LinkedList<IExpression>(flattened);

            bool requiredRecompute;
            do
            {
                requiredRecompute = false;

                var origin = candidates.First;
                while (origin != null)
                {
                    var current = origin.Next;
                    while (current != null)
                    {
                        // Idempotence / Commutative / Associative
                        if (origin.Value.Equals(current.Value))
                        {
                            candidates.Remove(current);
                            requiredRecompute = true;
                        }
                        // The pair are both type term.
                        else
                        {
                            switch (selector(origin.Value, current.Value, context))
                            {
                                case ChoiceResults.Equal:
                                case ChoiceResults.AcceptLeft:
                                    current.Value = origin.Value;
                                    requiredRecompute = true;
                                    break;
                                case ChoiceResults.AcceptRight:
                                    origin.Value = current.Value;
                                    requiredRecompute = true;
                                    break;
                            }
                        }

                        current = current.Next;
                    }

                    origin = origin.Next;
                }
            }
            while (requiredRecompute);

            return candidates;
        }

        private static IExpression? ReConstructExpression(
            IExpression[] results,
            Func<IExpression, IExpression, IExpression> creator) =>
            results.Length switch
            {
                0 => null,
                1 => results[0],
                2 => creator(results[0], results[1]),
                _ => results.Skip(2).Aggregate(creator(results[0], results[1]), creator)
            };

        public IExpression Compute(IExpression operand, TContext context)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = this.Compute(binary.Left, context);
                var right = this.Compute(binary.Right, context);

                if (binary is IAndExpression)
                {
                    // Absorption
                    var absorption =
                        this.ComputeAbsorption<OrFlattenedExpression>(left, right, this.ChoiceForAnd, context).
                        Memoize();
                    if (ReConstructExpression(absorption, OrExpression.Create) is IExpression result1)
                    {
                        return this.Compute(result1, context);
                    }

                    // Shrink
                    var shrinked =
                        this.ComputeShrink<IAndExpression>(left, right, this.ChoiceForAnd, context).
                        Memoize();
                    if (ReConstructExpression(shrinked, AndExpression.Create) is IExpression result2)
                    {
                        return result2;
                    }
                }
                else if (binary is IOrExpression)
                {
                    // Absorption
                    var absorption =
                        this.ComputeAbsorption<AndFlattenedExpression>(left, right, this.ChoiceForOr, context).
                        Memoize();
                    if (ReConstructExpression(absorption, AndExpression.Create) is IExpression result1)
                    {
                        return this.Compute(result1, context);
                    }

                    // Shrink
                    var shrinked =
                        this.ComputeShrink<IOrExpression>(left, right, this.ChoiceForOr, context).
                        Memoize();
                    if (ReConstructExpression(shrinked, OrExpression.Create) is IExpression result2)
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

        public static readonly LogicalCalculator<TContext> Instance =
            new LogicalCalculator<TContext>();
    }
}
