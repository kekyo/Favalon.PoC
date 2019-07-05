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

        protected abstract Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint);
        protected abstract Expression VisitResolving(IResolvingEnvironment environment);

        protected abstract FormattedString FormatReadableString(FormatContext context);

        public string ReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Standard, FormatNamings.Friendly, FormatOperators.Standard),
                this, false);
        public string StrictReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Always, FormatNamings.Standard, FormatOperators.Standard),
                this, false);

        public string FormatReadableString(
            FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
            bool encloseParenthesesIfRequired = false) =>
            FormatReadableString(new FormatContext(
                formatAnnotation, formatNaming, formatOperator), this, encloseParenthesesIfRequired);

        public override string ToString()
        {
            var name = this.GetType().Name.Replace("Expression", string.Empty);
            return $"{name}: {FormatReadableString(new FormatContext(FormatAnnotations.Standard, FormatNamings.Standard, FormatOperators.Standard), this, false)}";
        }
    }
}
