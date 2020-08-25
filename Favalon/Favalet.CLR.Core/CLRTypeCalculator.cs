using System;
using System.Reflection;
using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Internal;

namespace Favalet
{
    public sealed class CLRTypeCalculator :
        TypeCalculator
    {
        protected override ChoiceResults ChoiceForAnd(
            IExpression left, IExpression right, IResolver resolver)
        {
            // Narrowing
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (rt.IsAssignableFrom(lt))
                {
                    return ChoiceResults.AcceptLeft;
                }
                if (lt.IsAssignableFrom(rt))
                {
                    return ChoiceResults.AcceptRight;
                }
            }

            return base.ChoiceForAnd(left, right, resolver);
        }

        protected override ChoiceResults ChoiceForOr(
            IExpression left, IExpression right, IResolver resolver)
        {
            // Widening
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (lt.IsAssignableFrom(rt))
                {
                    return ChoiceResults.AcceptLeft;
                }
                if (rt.IsAssignableFrom(lt))
                {
                    return ChoiceResults.AcceptRight;
                }
            }

            return base.ChoiceForOr(left, right, resolver);
        }

        public new static readonly CLRTypeCalculator Instance =
            new CLRTypeCalculator();
    }
}
