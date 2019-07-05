using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Expression
    {
        protected static Expression Unify(
            Environment environment, Expression expression1, Expression expression2) =>
            environment.Unify(expression1, expression2);
        protected static Expression Unify(
            Environment environment, Expression expression1, Expression expression2, Expression expression3) =>
            environment.Unify(expression1, expression2, expression3);

        protected static Expression CreatePlaceholderIfRequired(
            Environment environment, Expression from) =>
            (from is UnspecifiedExpression) ? environment.CreatePlaceholder(UnspecifiedExpression.Instance) : from;

        protected static void Memoize(
            Environment environment, VariableExpression symbol, Expression expression) =>
            environment.Memoize(symbol, expression);

        protected static Expression? Lookup(
            Environment environment, VariableExpression symbol) =>
            environment.Lookup(symbol);

        protected internal static Expression VisitInferring(Environment environment, Expression expression, Expression higherOrderHint) =>
            expression.VisitInferring(environment, higherOrderHint);

        protected internal static Expression VisitResolving(Environment environment, Expression expression) =>
            expression.VisitResolving(environment);

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
                FormatAnnotations.Always => true,
                FormatAnnotations.Without => false,
                _ => (expression.HigherOrder != null) && !(expression.HigherOrder is PseudoExpression)
            };

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired) =>
            IsRequiredAnnotation(context, expression) ?
                $"{expression.FormatReadableString(context, true)}:{expression.HigherOrder.FormatReadableString(context.NewDerived(FormatAnnotations.Without, null, null), true)}" :
                expression.FormatReadableString(context, encloseParenthesesIfRequired);
    }
}
