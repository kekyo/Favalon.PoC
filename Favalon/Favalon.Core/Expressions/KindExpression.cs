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
            base(null!)
        { }

        public override string ReadableString =>
            "*";

        public override string ToString() =>
            $"Kind: *";

        protected override Expression VisitInferring(Environment environment, InferContext context) =>
            this;

        public static readonly KindExpression Instance = new KindExpression();
    }
}
