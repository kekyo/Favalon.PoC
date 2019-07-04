using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class KindExpression : PseudoExpression
    {
        private KindExpression()
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            "*";

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(Environment environment) =>
            this;

        public static readonly KindExpression Instance = new KindExpression();
    }
}
