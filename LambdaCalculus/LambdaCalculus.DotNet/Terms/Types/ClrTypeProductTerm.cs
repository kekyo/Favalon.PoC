using Favalon.Terms.Algebraic;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeProductTerm : ProductTerm
    {
        private ClrTypeProductTerm(Term lhs, Term rhs) :
            base(lhs, rhs, KindTerm.Instance)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ClrTypeProductTerm(lhs, rhs);

        public static ClrTypeProductTerm Create(Term lhs, Term rhs) =>
            new ClrTypeProductTerm(lhs, rhs);
    }
}
