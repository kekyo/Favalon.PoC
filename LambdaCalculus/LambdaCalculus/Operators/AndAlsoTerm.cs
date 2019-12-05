namespace Favalon.Operators
{
    public sealed class AndAlsoOperatorTerm : OperatorSymbolTerm<AndAlsoOperatorTerm>, IApplicable
    {
        private AndAlsoOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new AndAlsoLeftTerm(rhs);

        public static readonly AndAlsoOperatorTerm Instance =
            new AndAlsoOperatorTerm();

        private sealed class AndAlsoLeftTerm : OperatorArgument0Term<AndAlsoLeftTerm>, IApplicable
        {
            public AndAlsoLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new AndAlsoLeftTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                AndAlsoTerm.Reduce(context, this.Argument0, rhs);
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
                        return BooleanTerm.From(r.Value);
                    }
                    else
                    {
                        return new AndAlsoTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return BooleanTerm.False;
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
