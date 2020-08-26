using Favalet.Expressions.Algebraic;
using System;
using Favalet.Contexts;
using Favalet.Expressions;

namespace Favalet
{
    public interface ITypeCalculator
    {
        IExpression Compute(IExpression operand);
    }
    
    public class TypeCalculator :
        LogicalCalculator, ITypeCalculator
    {
        protected override ChoiceResults ChoiceForAnd(
            IExpression left, IExpression right)
        {
            // Function variance:
            if (left is IFunctionExpression(IExpression lp, IExpression lr) &&
                right is IFunctionExpression(IExpression rp, IExpression rr))
            {
                // Contravariance.
                switch (this.ChoiceForAnd(lp, rp), this.ChoiceForOr(lr, rr))
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

            return base.ChoiceForAnd(left, right);
        }

        protected override ChoiceResults ChoiceForOr(
            IExpression left, IExpression right)
        {
            // Function variance:
            if (left is IFunctionExpression(IExpression lp, IExpression lr) &&
                right is IFunctionExpression(IExpression rp, IExpression rr))
            {
                // Covariance.
                switch (this.ChoiceForOr(lp, rp), this.ChoiceForAnd(lr, rr))
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

            return base.ChoiceForOr(left, right);
        }

        public new static readonly TypeCalculator Instance =
            new TypeCalculator();
    }
}