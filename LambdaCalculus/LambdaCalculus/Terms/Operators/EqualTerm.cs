using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class EqualOperatorTerm : OperatorSymbolTerm<EqualOperatorTerm>, IApplicable
    {
        private EqualOperatorTerm()
        { }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
           this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            new EqualLeftTerm(argument);   // NOT reduced at this time.

        public static readonly EqualOperatorTerm Instance =
            new EqualOperatorTerm();

        private sealed class EqualLeftTerm : OperatorArgument0Term<EqualLeftTerm>, IApplicable
        {
            public EqualLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new EqualLeftTerm(argument);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
               this;

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
                this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                EqualTerm.Reduce(context, this.Argument0, argument);
        }
    }
}
