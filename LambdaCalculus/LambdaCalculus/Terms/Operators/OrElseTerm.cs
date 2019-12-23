using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class OrElseOperatorTerm : OperatorSymbolTerm<OrElseOperatorTerm>, IApplicable
    {
        private OrElseOperatorTerm()
        { }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
           this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            new OrElseLeftTerm(argument);   // NOT reduced at this time.

        public static readonly OrElseOperatorTerm Instance =
            new OrElseOperatorTerm();

        private sealed class OrElseLeftTerm : OperatorArgument0Term<OrElseLeftTerm>, IApplicable
        {
            public OrElseLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new OrElseLeftTerm(argument);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
               this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                OrElseTerm.Reduce(context, this.Argument0, argument);
        }
    }
}
