using Favalet.Expressions.Algebraic;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Security;

namespace Favalet.Expressions
{
    public sealed class TypeCalculator :
        LogicalCalculator
    {
        private enum ReduceResults
        {
            NonRelated,
            AcceptLeft,
            AcceptRight,
        }

        private static IEnumerable<IExpression> ReduceTypes(
            IEnumerable<IExpression> expressions,
            Func<ITypeTerm, ITypeTerm, ReduceResults> predicate)
        {
            var exprs = new LinkedList<IExpression>(expressions);

            bool requiredRecompute;
            do
            {
                requiredRecompute = false;

                var start = exprs.First;
                while (start != null)
                {
                    var current = start.Next;
                    while (current != null)
                    {
                        // Idempotence / Commutative / Associative
                        if (start.Value.Equals(current.Value))
                        {
                            exprs.Remove(current);
                            requiredRecompute = true;
                        }
                        // The pair are both type term.
                        else if (
                            start.Value is ITypeTerm leftType &&
                            current.Value is ITypeTerm rightType)
                        {
                            switch (predicate(leftType, rightType))
                            {
                                case ReduceResults.AcceptLeft:
                                    current.Value = leftType;
                                    requiredRecompute = true;
                                    break;
                                case ReduceResults.AcceptRight:
                                    start.Value = rightType;
                                    requiredRecompute = true;
                                    break;
                            }
                        }

                        current = current.Next;
                    }

                    start = start.Next;
                }
            }
            while (requiredRecompute);

            return exprs;
        }

        protected override IExpression? ChoiceForAnd(IExpression left, IExpression right)
        {
            // Narrowing
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (rt.IsAssignableFrom(lt))
                {
                    return left;
                }
                if (lt.IsAssignableFrom(rt))
                {
                    return right;
                }
            }

            return base.ChoiceForAnd(left, right);
        }

        protected override IExpression? ChoiceForOr(IExpression left, IExpression right)
        {
            // Widening
            if (left is ITypeTerm(Type lt) &&
                right is ITypeTerm(Type rt))
            {
                if (lt.IsAssignableFrom(rt))
                {
                    return left;
                }
                if (rt.IsAssignableFrom(lt))
                {
                    return right;
                }
            }

            return base.ChoiceForOr(left, right);
        }
    }
}
