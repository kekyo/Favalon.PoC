﻿using Favalet.Expressions.Algebraic;
using Favalet.Internal;
using System;
using System.Collections.Generic;

namespace Favalet.Expressions
{
    public sealed class TypeCalculator :
        LogicalCalculator
    {
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

        protected override ReduceResults ChoiceForAnd(
            IExpression left, IExpression right) =>
            (left is ITypeTerm(Type lt) && right is ITypeTerm(Type rt)) ?
                (lt.IsAssignableFrom(rt) ?
                    ReduceResults.AcceptRight :
                    rt.IsAssignableFrom(lt) ?
                        ReduceResults.AcceptLeft :
                        ReduceResults.NonRelated) :
                ReduceResults.NonRelated;

        protected override ReduceResults ChoiceForOr(
            IExpression left, IExpression right) =>
            (left is ITypeTerm(Type lt) && right is ITypeTerm(Type rt)) ?
                (lt.IsAssignableFrom(rt)?
                    ReduceResults.AcceptLeft :
                    rt.IsAssignableFrom(lt)?
                        ReduceResults.AcceptRight :
                        ReduceResults.NonRelated) :
                ReduceResults.NonRelated;

        // Narrow
        protected override IEnumerable<IExpression> ReduceForAnd(
            IEnumerable<IExpression> expressions) =>
            ReduceTypes(expressions, this.ChoiceForAnd);

        // Widen
        protected override IEnumerable<IExpression> ReduceForOr(
            IEnumerable<IExpression> expressions) =>
            ReduceTypes(expressions, this.ChoiceForOr);
    }
}
