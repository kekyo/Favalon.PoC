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

        private IExpression? ComputeAbsorption<TFlattenedExpression>(
            IExpression left,
            IExpression right,
            Func<IExpression, IExpression, IExpression?> predicate)
            where TFlattenedExpression : FlattenedExpression
        {
            var fl = Flatten(left);
            var fr = Flatten(right);

            if (fr is TFlattenedExpression(IExpression[] rightOperands))
            {
                foreach (var rightOperand in rightOperands)
                {
                    if (predicate(fl, rightOperand) is IExpression result)
                    {
                        return result;
                    }
                }
            }

            if (fl is TFlattenedExpression(IExpression[] leftOperands))
            {
                foreach (var leftOperand in leftOperands)
                {
                    if (predicate(leftOperand, fr) is IExpression result)
                    {
                        return right;
                    }
                }
            }

            return null;
        }

        protected virtual IExpression? ChoiceForAnd(IExpression left, IExpression right) =>
            // Idempotence
            this.Equals(left, right) ?
                left :
                null;

        protected virtual IExpression? ChoiceForOr(IExpression left, IExpression right) =>
            // Idempotence
            this.Equals(left, right) ?
                left :
                null;

        public IExpression Compute(IExpression operand)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = this.Compute(binary.Left);
                var right = this.Compute(binary.Right);

                if (binary is IAndBinaryExpression)
                {
                    if (this.ChoiceForAnd(left, right) is IExpression result)
                    {
                        return result;
                    }

                    // Absorption
                    if (this.ComputeAbsorption<OrFlattenedExpression>(
                        left,
                        right,
                        ChoiceForAnd) is IExpression computed1)
                    {
                        return computed1;
                    }
                }
                else if (binary is IOrBinaryExpression)
                {
                    if (this.ChoiceForOr(left, right) is IExpression result)
                    {
                        return result;
                    }

                    // Absorption
                    if (this.ComputeAbsorption<AndFlattenedExpression>(
                        left,
                        right,
                        ChoiceForOr) is IExpression computed2)
                    {
                        return computed2;
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
