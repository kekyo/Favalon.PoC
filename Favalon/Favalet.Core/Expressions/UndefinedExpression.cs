using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class UndefinedExpression : PseudoExpression
    {
        private UndefinedExpression()
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            "?";

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            throw new InvalidOperationException();

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
