using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class UndefinedExpression : TermExpression
    {
        private UndefinedExpression() :
            base(null!)
        { }

        public override string ReadableString =>
            "?";

        protected override Expression VisitInferring(Environment environment) =>
            this;

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
