namespace LambdaCalculus.Operators
{
    public sealed class OrElseOperatorTerm : OperatorSymbolTerm<OrElseOperatorTerm>, IApplicable
    {
        private OrElseOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new OrElseLeftTerm(rhs);

        public static readonly OrElseOperatorTerm Instance =
            new OrElseOperatorTerm();

        private sealed class OrElseLeftTerm : OperatorArgument0Term<OrElseLeftTerm>, IApplicable
        {
            public OrElseLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new OrElseLeftTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                OrElseTerm.Reduce(context, this.Argument0, rhs);
        }
    }

    public sealed class OrElseTerm : LogicalBinaryOperatorTerm<OrElseTerm>
    {
        internal OrElseTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        protected override Term Create(Term lhs, Term rhs) =>
            new OrElseTerm(lhs, rhs);

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Reduce(context);
            if (lhs_ is BooleanTerm l)
            {
                if (!l.Value)
                {
                    var rhs_ = rhs.Reduce(context);
                    if (rhs_ is BooleanTerm r)
                    {
                        return Constant(r.Value);
                    }
                    else
                    {
                        return new OrElseTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return BooleanTerm.True;
                }
            }
            else
            {
                return new OrElseTerm(lhs_, rhs);
            }
        }

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);
    }
}
