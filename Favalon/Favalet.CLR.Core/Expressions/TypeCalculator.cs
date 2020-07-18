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
        private static IEnumerable<IExpression> MakeSimple(
            IEnumerable<IExpression> expressions,
            Func<ITypeTerm, ITypeTerm, ITypeTerm?> predicate) =>
            expressions.Aggregate(
                ArrayEx.Empty<IExpression>(),
                (calculatedExprs, expr) =>
                {
                    if (expr is ITypeTerm exprType)
                    {
                        return calculatedExprs.
                            Select(calculatedExpr => predicate(calculatedExpr, exprType) ?? ).
                            Memoize();
                    }
                    else
                    {
                        return calculatedExprs.
                            Append(expr).
                            Distinct().
                            Memoize();
                    }
                });

        protected override IEnumerable<IExpression> MakeSimpleOr(
            IEnumerable<IExpression> expressions)
        {
            // Widen
            var widened = MakeSimple(
                base.MakeSimpleOr(expressions),
                (calculatedExpr, expr) =>
                    calculatedExpr.RuntimeType.IsAssignableFrom(expr.RuntimeType) ?
                        calculatedExpr :
                        expr.RuntimeType.IsAssignableFrom(calculatedExpr.RuntimeType) ?
                            expr :
                            null);

            return widened;
        }

        protected override IEnumerable<IExpression> MakeSimpleAnd(
            IEnumerable<IExpression> expressions)
        {
            // Narrow
            var narrowed = MakeSimple(
                base.MakeSimpleAnd(expressions),
                (calculatedExpr, expr) =>
                    calculatedExpr.RuntimeType.IsAssignableFrom(expr.RuntimeType) ?
                        expr :
                        expr.RuntimeType.IsAssignableFrom(calculatedExpr.RuntimeType) ?
                            calculatedExpr :
                            null);

            return narrowed;
        }
    }
}
