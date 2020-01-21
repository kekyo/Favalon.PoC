using Favalon.Terms.Algebraic;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeSumTerm : SumTerm
    {
        private ClrTypeSumTerm(Term lhs, Term rhs) :
            base(lhs, rhs, KindTerm.Instance)
        { }

        protected override Term OnCreate(Term lhs, Term rhs, Term higherOrder) =>
            new ClrTypeSumTerm(lhs, rhs);

        public static ClrTypeSumTerm Create(Term lhs, Term rhs) =>
            new ClrTypeSumTerm(lhs, rhs);
    }
}
