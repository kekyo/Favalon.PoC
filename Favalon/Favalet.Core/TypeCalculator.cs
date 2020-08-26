using Favalet.Expressions.Algebraic;
using System;
using Favalet.Contexts;
using Favalet.Expressions;

namespace Favalet
{
    public interface ITypeCalculator
    {
        IExpression Compute(IExpression operand, IPlaceholderResolver resolver);
    }
    
    public class TypeCalculator :
        LogicalCalculator<IPlaceholderResolver>, ITypeCalculator
    {
        protected override ChoiceResults ChoiceForAnd(
            IExpression left, IExpression right, IPlaceholderResolver resolver)
        {
            var l = resolver.UnsafeResolveWhile(left);
            var r = resolver.UnsafeResolveWhile(right);
            
            // Function variance:
            if (l is IFunctionExpression(IExpression lp, IExpression lr) &&
                r is IFunctionExpression(IExpression rp, IExpression rr))
            {
                // Contravariance.
                switch (this.ChoiceForAnd(lp, rp, resolver), this.ChoiceForOr(lr, rr, resolver))
                {
                    case (ChoiceResults.AcceptLeft, ChoiceResults.Equal):
                    case (ChoiceResults.AcceptLeft, ChoiceResults.AcceptLeft):
                        return ChoiceResults.AcceptLeft;
                    case (ChoiceResults.AcceptRight, ChoiceResults.Equal):
                    case (ChoiceResults.AcceptRight, ChoiceResults.AcceptRight):
                        return ChoiceResults.AcceptRight;
                    case (ChoiceResults.Equal, ChoiceResults.Equal):
                        return ChoiceResults.Equal;
                }
            }

            return base.ChoiceForAnd(left, right, resolver);
        }

        protected override ChoiceResults ChoiceForOr(
            IExpression left, IExpression right, IPlaceholderResolver resolver)
        {
            var l = resolver.UnsafeResolveWhile(left);
            var r = resolver.UnsafeResolveWhile(right);
            
            // Function variance:
            if (l is IFunctionExpression(IExpression lp, IExpression lr) &&
                r is IFunctionExpression(IExpression rp, IExpression rr))
            {
                // Covariance.
                switch (this.ChoiceForOr(lp, rp, resolver), this.ChoiceForAnd(lr, rr, resolver))
                {
                    case (ChoiceResults.AcceptLeft, ChoiceResults.Equal):
                    case (ChoiceResults.AcceptLeft, ChoiceResults.AcceptLeft):
                        return ChoiceResults.AcceptLeft;
                    case (ChoiceResults.AcceptRight, ChoiceResults.Equal):
                    case (ChoiceResults.AcceptRight, ChoiceResults.AcceptRight):
                        return ChoiceResults.AcceptRight;
                    case (ChoiceResults.Equal, ChoiceResults.Equal):
                        return ChoiceResults.Equal;
                }
            }

            return base.ChoiceForOr(left, right, resolver);
        }

        public new static readonly TypeCalculator Instance =
            new TypeCalculator();
    }
}