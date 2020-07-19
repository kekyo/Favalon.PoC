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
                new[] { left };

            var rf = right is TBinaryExpression rb ?
                Flatten<TBinaryExpression>(rb.Left, rb.Right) :
                new[] { right };

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

        private static IEnumerable<IExpression> FlattenAll<TBinaryExpression>(
            IExpression left, IExpression right)
            where TBinaryExpression : IBinaryExpression
        {
            var lf = left is TBinaryExpression lb ?
                FlattenAll<TBinaryExpression>(lb.Left, lb.Right) :
                new[] { FlattenAll(left) };

            var rf = right is TBinaryExpression rb ?
                FlattenAll<TBinaryExpression>(rb.Left, rb.Right) :
                new[] { FlattenAll(right) };

            return lf.Concat(rf);
        }

        private static IExpression FlattenAll(IExpression expression) =>
            expression switch
            {
                IAndBinaryExpression and => new AndFlattenedExpression(
                    FlattenAll<IAndBinaryExpression>(and.Left, and.Right).Memoize()),
                IOrBinaryExpression or => new OrFlattenedExpression(
                    FlattenAll<IOrBinaryExpression>(or.Left, or.Right).Memoize()),
                _ => expression
            };

        public bool Equals(IExpression lhs, IExpression rhs)
        {
            var left = FlattenAll(lhs);
            var right = FlattenAll(rhs);

            return left.Equals(right);
        }

        private IExpression? ComputeAbsorption<TFlattenedExpression>(
            IExpression left, IExpression right)
            where TFlattenedExpression : FlattenedExpression
        {
            if (Flatten(right) is TFlattenedExpression(IExpression[] operands1))
            {
                foreach (var operand in operands1)
                {
                    if (this.Equals(left, operand))
                    {
                        return left;
                    }
                }
            }

            if (Flatten(left) is TFlattenedExpression(IExpression[] operands2))
            {
                foreach (var operand in operands2)
                {
                    if (this.Equals(operand, right))
                    {
                        return right;
                    }
                }
            }

            return null;
        }

        public IExpression Compute(IExpression operand)
        {
            if (operand is IBinaryExpression binary)
            {
                var left = this.Compute(binary.Left);
                var right = this.Compute(binary.Right);

                // Idempotence
                if (this.Equals(left, right))
                {
                    return left;
                }

                // Absorption
                if (binary is IAndBinaryExpression &&
                    this.ComputeAbsorption<OrFlattenedExpression>(left, right) is IExpression computed1)
                {
                    return computed1;
                }
                else if (binary is IOrBinaryExpression &&
                    this.ComputeAbsorption<AndFlattenedExpression>(left, right) is IExpression computed2)
                {
                    return computed2;
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
