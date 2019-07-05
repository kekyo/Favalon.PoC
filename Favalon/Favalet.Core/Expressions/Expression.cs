using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract partial class Expression
    {
        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public readonly Expression HigherOrder;

        protected abstract Expression VisitInferring(Environment environment, Expression higherOrderHint);
        protected abstract Expression VisitResolving(Environment environment);

        protected abstract FormattedString FormatReadableString(FormatContext context);

        public string ReadableString =>
            FormatReadableString(new FormatContext(FormatAnnotations.SuppressPseudoes, FormatNamings.Standard), this, false);
        public string StrictReadableString =>
            FormatReadableString(new FormatContext(FormatAnnotations.Strict, FormatNamings.Strict), this, false);

        public override string ToString()
        {
            var name = this.GetType().Name.Replace("Expression", string.Empty);
            return $"{name}: {FormatReadableString(new FormatContext(FormatAnnotations.SuppressPseudoes, FormatNamings.Strict), this, false)}";
        }
    }
}
