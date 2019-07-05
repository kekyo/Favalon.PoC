using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    partial class Expression
    {
        protected internal interface IInferringEnvironment
        {
            Expression Unify(Expression expression1, Expression expression2);
            Expression Unify(Expression expression1, Expression expression2, Expression expression3);

            Expression CreatePlaceholderIfRequired(Expression from);

            void Memoize(VariableExpression symbol, Expression expression);

            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression, Expression higherOrderHint)
                where TExpression : Expression;
        }

        protected internal interface IResolvingEnvironment
        {
            Expression? Lookup(VariableExpression symbol);

            TExpression Visit<TExpression>(TExpression expression)
                where TExpression : Expression;
        }

        internal Expression InternalVisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferring(environment, higherOrderHint);
        internal Expression InternalVisitResolving(IResolvingEnvironment environment) =>
            this.VisitResolving(environment);

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
