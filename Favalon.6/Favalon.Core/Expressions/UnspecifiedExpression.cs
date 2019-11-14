using System;

namespace Favalon.Expressions
{
    public sealed class UnspecifiedExpression : Expression
    {
        private UnspecifiedExpression()
        { }

        public override Expression HigherOrder =>
            throw new NotImplementedException();

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is UnspecifiedExpression;

        public override string ToString() =>
            "_";

        public static readonly Expression Instance = new UnspecifiedExpression();
    }
}
