using Favalet.Expressions.Algebraic;
using Favalet.Internal;
using System;

namespace Favalet.Expressions
{
    public sealed class TypeCalculator :
        LogicalCalculator
    {
        protected override ReduceResults ChoiceForAnd(
            IExpression left, IExpression right)
        {
            // Narrowing
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (rt.IsAssignableFrom(lt))
                {
                    return ReduceResults.AcceptLeft;
                }
                if (lt.IsAssignableFrom(rt))
                {
                    return ReduceResults.AcceptRight;
                }
            }

            return base.ChoiceForAnd(left, right);
        }

        protected override ReduceResults ChoiceForOr(
            IExpression left, IExpression right)
        {
            // Widening
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (lt.IsAssignableFrom(rt))
                {
                    return ReduceResults.AcceptLeft;
                }
                if (rt.IsAssignableFrom(lt))
                {
                    return ReduceResults.AcceptRight;
                }
            }

            return base.ChoiceForOr(left, right);
        }
    }
}
