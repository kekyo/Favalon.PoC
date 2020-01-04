using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class AndAlsoOperatorTerm : OperatorSymbolTerm<AndAlsoOperatorTerm>, IApplicable
    {
        private AndAlsoOperatorTerm()
        { }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            new AndAlsoLeftTerm(argument);  // NOT reduced at this time.

        public static readonly AndAlsoOperatorTerm Instance =
            new AndAlsoOperatorTerm();

        private sealed class AndAlsoLeftTerm : OperatorArgument0Term<AndAlsoLeftTerm>, IApplicable
        {
            public AndAlsoLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new AndAlsoLeftTerm(argument);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
               this;

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
                this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                AndAlsoTerm.Reduce(context, this.Argument0, argument);
        }
    }
}
