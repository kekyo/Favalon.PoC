using Favalon.Terms;
using System;

namespace Favalon.Expressions
{
    public sealed class UnknownTermExpression : Expression
    {
        public readonly Term Term;

        public UnknownTermExpression(Term term) =>
            this.Term = term;

        public override Expression HigherOrder =>
            throw new NotImplementedException();

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is UnknownTermExpression unknown ?
                this.Term.Equals(unknown.Term) :
                false;

        public override string ToString() =>
            $"Uninterrupted({this.Term})";
    }
}
