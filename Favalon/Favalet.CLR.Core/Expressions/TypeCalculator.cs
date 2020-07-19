using Favalet.Expressions.Algebraic;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

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
                        if (start.Value.Equals(current.Value))
                        {
                            exprs.Remove(current);
                            requiredRecompute = true;
                        }
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

        protected override IEnumerable<IExpression> ReduceByOr(
            IEnumerable<IExpression> expressions)
        {
            // Widen
            var widened = ReduceTypes(
                expressions,
                (calculatedExpr, expr) =>
                    calculatedExpr.RuntimeType.IsAssignableFrom(expr.RuntimeType) ?
                        ReduceResults.AcceptLeft :
                        expr.RuntimeType.IsAssignableFrom(calculatedExpr.RuntimeType) ?
                            ReduceResults.AcceptRight :
                            ReduceResults.NonRelated);

            return widened;
        }

        protected override IEnumerable<IExpression> ReduceByAnd(
            IEnumerable<IExpression> expressions)
        {
            // Narrow
            var narrowed = ReduceTypes(
                expressions,
                (calculatedExpr, expr) =>
                    calculatedExpr.RuntimeType.IsAssignableFrom(expr.RuntimeType) ?
                        ReduceResults.AcceptRight :
                        expr.RuntimeType.IsAssignableFrom(calculatedExpr.RuntimeType) ?
                            ReduceResults.AcceptLeft :
                            ReduceResults.NonRelated);

            return narrowed;
        }
    }
}
