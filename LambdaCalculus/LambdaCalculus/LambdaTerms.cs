namespace LambdaCalculus
{
    public sealed class LambdaOperatorTerm : ApplicableTerm
    {
        private LambdaOperatorTerm()
        { }

        public override Term HigherOrder =>
            LambdaCalculus.UnspecifiedTerm.Instance;

        public override Term Reduce(ReduceContext context) =>
            this;

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new LambdaArrowParameterTerm(rhs.Reduce(context));

        public override Term Infer(InferContext context) =>
            this;

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
                LambdaCalculus.UnspecifiedTerm.Instance;

            public override Term Reduce(ReduceContext context) =>
                new LambdaArrowParameterTerm(this.Parameter.Reduce(context));

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                new LambdaTerm(this.Parameter, rhs);    // rhs isn't reduced at this time, because the body term can reduce only applying time.

            public override Term Infer(InferContext context) =>
                new LambdaArrowParameterTerm(this.Parameter.Infer(context));

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

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();
            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                newScope.AddBoundTerm(identity.Identity, parameter);
            }

            var body = this.Body.Infer(newScope);

            return new LambdaTerm(parameter, body);
        }

        public override Term Fixup(InferContext context) =>
            new LambdaTerm(this.Parameter.Fixup(context), this.Body.Fixup(context));

        public override bool Equals(Term? other) =>
            other is LambdaTerm rhs ?
                (this.Parameter.Equals(rhs.Parameter) && this.Body.Equals(rhs.Body)) :
                false;

        public void Deconstruct(out Term parameter, out Term body)
        {
            parameter = this.Parameter;
            body = this.Body;
        }

        public static readonly LambdaTerm Unspecified =
            new LambdaTerm(LambdaCalculus.UnspecifiedTerm.Instance, LambdaCalculus.UnspecifiedTerm.Instance);
    }
}
