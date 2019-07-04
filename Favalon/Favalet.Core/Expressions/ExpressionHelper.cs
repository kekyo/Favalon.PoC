using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Expression
    {
        protected Expression CreatePlaceholderIfRequired(Environment environment, Expression expressionHint, Expression higherOrder) =>
            (expressionHint is UndefinedExpression) ?
                environment.CreatePlaceholder(higherOrder) :
                expressionHint;

        protected internal static Expression VisitInferring(Environment environment, Expression expression, Expression higherOrderHint) =>
            expression.VisitInferring(environment, higherOrderHint);

        private string FormatReadableString(FormatContext context, bool encloseParenthesesIfRequired)
        {
            var result = this.FormatReadableString(context);
            return (encloseParenthesesIfRequired && result.RequiredEnclosingParentheses) ?
                $"({result.Formatted})" :
                result.Formatted;
        }

        private static bool IsRequiredAnnotation(FormatContext context, Expression expression)
        {
            switch (context.FormatAnnotation)
            {
                case FormatAnnotations.Strict:
                    return true;
                case FormatAnnotations.Without:
                    return false;
                default:
                    return !(expression.HigherOrder is PseudoExpression);
            }
        }

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired) =>
            IsRequiredAnnotation(context, expression) ?
                $"{expression.FormatReadableString(context, true)}:{expression.HigherOrder.FormatReadableString(context.NewDerived(FormatAnnotations.Without, null), true)}" :
                expression.FormatReadableString(context, encloseParenthesesIfRequired);
    }
}
