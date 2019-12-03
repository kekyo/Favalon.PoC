namespace LambdaCalculus
{
    public sealed class LambdaOperatorTerm : ApplicableTerm
    {
        private LambdaOperatorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override Term Reduce(ReduceContext context) =>
            this;

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new LambdaArrowParameterTerm(rhs.Reduce(context));

        public override Term Infer(InferContext context) =>
            this;

        protected internal override Term? InferForApply(InferContext context, Term rhs) =>
            new LambdaArrowParameterTerm(rhs.Infer(context));

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is LambdaOperatorTerm;

        public static LambdaOperatorTerm Instance =
            new LambdaOperatorTerm();

        private sealed class LambdaArrowParameterTerm : ApplicableTerm
        {
            public readonly Term Parameter;

            public LambdaArrowParameterTerm(Term parameter) =>
                this.Parameter = parameter;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(ReduceContext context) =>
                new LambdaArrowParameterTerm(this.Parameter.Reduce(context));

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                new LambdaTerm(this.Parameter, rhs);    // rhs isn't reduced at this time, because the body term can reduce only applying time.

            public override Term Infer(InferContext context) =>
                new LambdaArrowParameterTerm(this.Parameter.Infer(context));

            protected internal override Term? InferForApply(InferContext context, Term rhs) =>
                new LambdaTerm(this.Parameter, rhs);    // rhs isn't inferred at this time, because lack parameter bound information.

            public override Term Fixup(InferContext context) =>
                new LambdaArrowParameterTerm(this.Parameter.Fixup(context));

            public override bool Equals(Term? other) =>
                other is LambdaArrowParameterTerm rhs ? this.Parameter.Equals(rhs.Parameter) : false;
        }
    }

    public sealed class LambdaTerm : ApplicableTerm
    {
        public readonly Term Parameter;
        public readonly Term Body;

        internal LambdaTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term HigherOrder =>
            new LambdaTerm(this.Parameter.HigherOrder, this.Body.HigherOrder);

        public override Term Reduce(ReduceContext context) =>
            new LambdaTerm(this.Parameter.Reduce(context), this.Body.Reduce(context));

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.AddBoundTerm(((IdentityTerm)this.Parameter).Identity, rhs);

            return this.Body.Reduce(newScope);
        }

        public override Term Infer(InferContext context) =>
            new LambdaTerm(this.Parameter.Infer(context), this.Body.Infer(context));

        protected internal override Term? InferForApply(InferContext context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.AddBoundTerm(((IdentityTerm)this.Parameter).Identity, rhs);

            return new LambdaTerm(this.Parameter, this.Body.Infer(newScope));
        }

        public override Term Fixup(InferContext context) =>
            new LambdaTerm(this.Parameter.Fixup(context), this.Body.Fixup(context));

        public override bool Equals(Term? other) =>
            other is LambdaTerm rhs ?
                (this.Parameter.Equals(rhs.Parameter) && this.Body.Equals(rhs.Body)) :
                false;
    }
}
