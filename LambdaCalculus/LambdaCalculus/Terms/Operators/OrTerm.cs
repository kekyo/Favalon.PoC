namespace Favalon.Terms.Operators
{
    public sealed class OrTerm : BinaryOperatorTerm<OrTerm>
    {
        internal OrTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        protected override Term Create(Term lhs, Term rhs, Term higherOrder) =>
            new OrTerm(lhs, rhs, higherOrder);

        public override Term Reduce(ReduceContext context) =>
            this;
    }
}
