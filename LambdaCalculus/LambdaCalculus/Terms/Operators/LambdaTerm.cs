using Favalon.Contexts;

namespace Favalon.Terms.Operators
{
    public sealed class LambdaOperatorTerm : OperatorSymbolTerm<LambdaOperatorTerm>, IApplicable
    {
        private LambdaOperatorTerm()
        { }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
           this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            new LambdaArrowParameterTerm(argument);   // NOT reduced at this time.

        public static LambdaOperatorTerm Instance =
            new LambdaOperatorTerm();

        private sealed class LambdaArrowParameterTerm : OperatorArgument0Term<LambdaArrowParameterTerm>, IApplicable
        {
            public LambdaArrowParameterTerm(Term parameter) :
                base(parameter)
            { }

            protected override Term Create(Term argument) =>
                new LambdaArrowParameterTerm(argument);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint) =>
               this;

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
                this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                LambdaTerm.Create(this.Argument0, argument);
        }
    }
}
