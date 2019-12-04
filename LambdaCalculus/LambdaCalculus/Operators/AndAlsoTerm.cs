namespace LambdaCalculus.Operators
{
    public sealed class AndAlsoOperatorTerm : ApplicableTerm
    {
        private AndAlsoOperatorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new AndAlsoLeftTerm(rhs.Reduce(context));

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is AndAlsoOperatorTerm;

        public static readonly AndAlsoOperatorTerm Instance =
            new AndAlsoOperatorTerm();

        private sealed class AndAlsoLeftTerm : ApplicableTerm
        {
            public readonly Term Lhs;

            public AndAlsoLeftTerm(Term lhs) =>
                this.Lhs = lhs;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(ReduceContext context) =>
                this;

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                AndAlsoTerm.Reduce(context, this.Lhs, rhs);

            public override Term Infer(InferContext context) =>
                new AndAlsoLeftTerm(this.Lhs.Infer(context));

            public override Term Fixup(InferContext context) =>
                new AndAlsoLeftTerm(this.Lhs.Fixup(context));

            public override bool Equals(Term? other) =>
                other is AndAlsoLeftTerm rhs ? this.Lhs.Equals(rhs.Lhs) : false;
        }
    }

    public sealed class AndAlsoTerm : LogicalBinaryOperatorTerm<AndAlsoTerm>
    {
        internal AndAlsoTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        protected override Term Create(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Reduce(context);
            if (lhs_ is BooleanTerm l)
            {
                if (l.Value)
                {
                    var rhs_ = rhs.Reduce(context);
                    if (rhs_ is BooleanTerm r)
                    {
                        return Constant(l.Value && r.Value);
                    }
                    else
                    {
                        return new AndAlsoTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return False();
                }
            }
            else
            {
                return new AndAlsoTerm(lhs_, rhs);
            }
        }

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);
    }
}
