using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Expression
    {
        protected static Expression CreatePlaceholderIfRequired(
            Environment environment, Expression expressionHint1, Expression expressionHint2, Expression higherOrder) =>
            (expressionHint2 is UndefinedExpression) ? ((expressionHint1 is UndefinedExpression) ? environment.CreatePlaceholder(higherOrder) : expressionHint1) :
            !(expressionHint1 is UndefinedExpression) ?
                throw new ArgumentException($"Cannot unifying: between \"{expressionHint1.ReadableString}\" and \"{expressionHint2.ReadableString}\"") :
                expressionHint2;
        protected static Expression CreatePlaceholderIfRequired(
            Environment environment, Expression expressionHint, Expression higherOrder) =>
            (expressionHint is UndefinedExpression) ? environment.CreatePlaceholder(higherOrder) : expressionHint;

        protected internal static Expression VisitInferring(Environment environment, Expression expression, Expression higherOrderHint) =>
            expression.VisitInferring(environment, higherOrderHint);

        private string FormatReadableString(FormatContext context, bool encloseParenthesesIfRequired)
        {
            var result = this.FormatReadableString(context);
            return (encloseParenthesesIfRequired && result.RequiredEnclosingParentheses) ?
                $"({result.Formatted})" :
                result.Formatted;
        }

        private static bool IsRequiredAnnotation(FormatContext context, Expression expression) =>
            context.FormatAnnotation switch
            {
                FormatAnnotations.Strict => true,
                FormatAnnotations.Without => false,
                _ => (expression.HigherOrder != null) && !(expression.HigherOrder is PseudoExpression)
            };

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired) =>
            IsRequiredAnnotation(context, expression) ?
                $"{expression.FormatReadableString(context, true)}:{expression.HigherOrder.FormatReadableString(context.NewDerived(FormatAnnotations.Without, null), true)}" :
                expression.FormatReadableString(context, encloseParenthesesIfRequired);
    }
}
