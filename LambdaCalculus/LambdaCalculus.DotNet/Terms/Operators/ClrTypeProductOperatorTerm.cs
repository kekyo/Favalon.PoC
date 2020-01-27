using Favalon.Terms.Contexts;
using Favalon.Terms.Types;

namespace Favalon.Terms.Operators
{
    public sealed class ClrTypeProductOperatorTerm : ProductOperatorTerm<ClrTypeCalculator>
    {
        private ClrTypeProductOperatorTerm() :
            base(ClrTypeCalculator.Instance)
        { }

        protected override AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new ClrTypeProductOperatorClosureTerm(argument),
                argument);

        public static readonly ClrTypeProductOperatorTerm Instance =
            new ClrTypeProductOperatorTerm();
    }

    internal sealed class ClrTypeProductOperatorClosureTerm : ProductOperatorClosureTerm<ClrTypeCalculator>
    {
        public ClrTypeProductOperatorClosureTerm(Term lhs) :
            base(ClrTypeCalculator.Instance, lhs)
        { }

        protected override Term OnCreate(ClrTypeCalculator calculator, Term lhs) =>
            new ClrTypeProductOperatorClosureTerm(lhs);

        protected override Term OnCreateTerm(Term lhs, Term rhs, Term higherOrder) =>
            ClrTypeProductTerm.Create(lhs, rhs);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeProductOperatorClosureTerm term ? this.Lhs.Equals(term.Lhs) : false;
    }
}
