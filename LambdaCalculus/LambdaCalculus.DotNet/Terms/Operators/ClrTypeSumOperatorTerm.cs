using Favalon.Terms.Contexts;
using Favalon.Terms.Types;

namespace Favalon.Terms.Operators
{
    public sealed class ClrTypeSumOperatorTerm : SumOperatorTerm<ClrTypeCalculator>
    {
        private ClrTypeSumOperatorTerm() :
            base(ClrTypeCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new ClrTypeSumOperatorClosureTerm(argument),
                argument);

        public static readonly ClrTypeSumOperatorTerm Instance =
            new ClrTypeSumOperatorTerm();
    }

    internal sealed class ClrTypeSumOperatorClosureTerm : SumOperatorClosureTerm<ClrTypeCalculator>
    {
        public ClrTypeSumOperatorClosureTerm(Term lhs) :
            base(ClrTypeCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(ClrTypeCalculator calculator, Term lhs) =>
            new ClrTypeSumOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            ClrTypeSumTerm.Create(lhs, rhs);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeSumOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
