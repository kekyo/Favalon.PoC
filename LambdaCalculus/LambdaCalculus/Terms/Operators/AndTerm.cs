using Favalon.Contexts;

namespace Favalon.Terms.Operators
{
    public sealed class AndTerm : BinaryOperatorTerm<AndTerm>
    {
        internal AndTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        protected override Term Create(Term lhs, Term rhs, Term higherOrder) =>
            new AndTerm(lhs, rhs, higherOrder);

        public override Term Reduce(ReduceContext context) =>
            this;
    }
}
