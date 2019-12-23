using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class NotOperatorTerm : OperatorSymbolTerm<NotOperatorTerm>, IApplicable
    {
        private NotOperatorTerm()
        { }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
           this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            NotTerm.Reduce(context, argument);

        public static readonly NotOperatorTerm Instance =
            new NotOperatorTerm();
    }
}
