namespace Favalon.Terms.Types
{
    public sealed class ClrTypeProductTerm : TypeProductTerm
    {
        private ClrTypeProductTerm(Term lhs, Term rhs) :
            base(lhs, rhs, KindTerm.Instance, ClrTypeCalculator.Instance)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ClrTypeProductTerm(lhs, rhs);

        public static ClrTypeProductTerm Create(Term lhs, Term rhs) =>
            new ClrTypeProductTerm(lhs, rhs);
    }
}
