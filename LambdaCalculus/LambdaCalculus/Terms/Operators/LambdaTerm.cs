namespace Favalon.Terms.Operators
{
    public sealed class LambdaOperatorTerm : OperatorSymbolTerm<LambdaOperatorTerm>, IApplicable
    {
        private LambdaOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new LambdaArrowParameterTerm(rhs);

        public static LambdaOperatorTerm Instance =
            new LambdaOperatorTerm();

        private sealed class LambdaArrowParameterTerm : OperatorArgument0Term<LambdaArrowParameterTerm>, IApplicable
        {
            public LambdaArrowParameterTerm(Term parameter) :
                base(parameter)
            { }

            protected override Term Create(Term argument) =>
                new LambdaArrowParameterTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                LambdaTerm.Create(this.Argument0, rhs);    // rhs isn't reduced at this time, because the body term can reduce only applying time.
        }
    }
}
