using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class NotOperatorTerm : OperatorSymbolTerm<NotOperatorTerm>, IApplicable
    {
        private NotOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            NotTerm.Reduce(context, rhs);

        public static readonly NotOperatorTerm Instance =
            new NotOperatorTerm();
    }
}
