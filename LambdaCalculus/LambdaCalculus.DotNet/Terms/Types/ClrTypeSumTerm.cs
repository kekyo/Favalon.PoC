namespace Favalon.Terms.Types
{
    public sealed class ClrTypeSumTerm : TypeSumTerm
    {
        private ClrTypeSumTerm(Term lhs, Term rhs) :
            base(lhs, rhs, KindTerm.Instance, ClrTypeCalculator.Instance)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ClrTypeSumTerm(lhs, rhs);

        public static ClrTypeSumTerm Create(Term lhs, Term rhs) =>
            new ClrTypeSumTerm(lhs, rhs);
    }
}
