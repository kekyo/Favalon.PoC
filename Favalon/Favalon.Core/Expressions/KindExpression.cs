using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class KindExpression : TermExpression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        public override string ReadableString =>
            "*";

        protected override Expression VisitInferring(Environment environment) =>
            this;

        public static readonly KindExpression Instance = new KindExpression();
    }
}
