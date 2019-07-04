using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class Expression
    {
        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public readonly Expression HigherOrder;

        protected abstract FormattedString FormatReadableString(FormatContext context);

        private string FormatReadableString(FormatContext context, bool encloseParenthesesIfRequired)
        {
            var result = this.FormatReadableString(context);
            return (encloseParenthesesIfRequired && result.RequiredEncloseParentheses) ?
                $"({result.Formatted})" :
                result.Formatted;
        }

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired) =>
            (context.WithAnnotation && expression.HigherOrder is Expression higherOrder) ?
                $"{expression.FormatReadableString(context, true)}:{higherOrder.FormatReadableString(context.NewDerived(false, null), true)}" :
                expression.FormatReadableString(context, encloseParenthesesIfRequired);

        public string ReadableString =>
            FormatReadableString(new FormatContext(true, FormatNamings.Standard), this, false);

        public override string ToString()
        {
            var name = this.GetType().Name.Replace("Expression", string.Empty);
            return $"{name}: {this.ReadableString}";
        }
    }
}
