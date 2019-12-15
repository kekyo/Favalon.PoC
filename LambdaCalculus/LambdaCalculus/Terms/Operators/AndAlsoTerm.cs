using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class AndAlsoOperatorTerm : OperatorSymbolTerm<AndAlsoOperatorTerm>, IApplicable
    {
        private AndAlsoOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new AndAlsoLeftTerm(rhs);

        public static readonly AndAlsoOperatorTerm Instance =
            new AndAlsoOperatorTerm();

        private sealed class AndAlsoLeftTerm : OperatorArgument0Term<AndAlsoLeftTerm>, IApplicable
        {
            public AndAlsoLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new AndAlsoLeftTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                AndAlsoTerm.Reduce(context, this.Argument0, rhs);
        }
    }
}
