using Favalon.Expressions;
using System;

namespace Favalon.Terms
{
    public sealed class UnspecifiedTerm : Term
    {
        private UnspecifiedTerm()
        { }

        protected override Expression VisitInfer(IInferContext context) =>
            UnspecifiedExpression.Instance;

        public override bool Equals(Term? rhs) =>
            rhs is UnspecifiedTerm;

        public override string ToString() =>
            "_";

        public static readonly Term Instance = new UnspecifiedTerm();
    }
}
