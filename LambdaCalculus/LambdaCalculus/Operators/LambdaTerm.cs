namespace LambdaCalculus.Operators
{
    public sealed class LambdaOperatorTerm : OperatorSymbolTerm<LambdaOperatorTerm>
    {
        private LambdaOperatorTerm()
        { }

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new LambdaArrowParameterTerm(rhs);

        public static LambdaOperatorTerm Instance =
            new LambdaOperatorTerm();

        private sealed class LambdaArrowParameterTerm : OperatorArgument0Term<LambdaArrowParameterTerm>
        {
            public LambdaArrowParameterTerm(Term parameter) :
                base(parameter)
            { }

            protected override Term Create(Term argument) =>
                new LambdaArrowParameterTerm(argument);

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                new LambdaTerm(this.Argument0, rhs);    // rhs isn't reduced at this time, because the body term can reduce only applying time.
        }
    }
}
