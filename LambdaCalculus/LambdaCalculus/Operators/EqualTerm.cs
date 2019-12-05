namespace Favalon.Operators
{
    public sealed class EqualOperatorTerm : OperatorSymbolTerm<EqualOperatorTerm>, IApplicable
    {
        private EqualOperatorTerm()
        { }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new EqualLeftTerm(rhs);

        public static readonly EqualOperatorTerm Instance =
            new EqualOperatorTerm();

        private sealed class EqualLeftTerm : OperatorArgument0Term<EqualLeftTerm>, IApplicable
        {
            public EqualLeftTerm(Term lhs) :
                base(lhs)
            { }

            protected override Term Create(Term argument) =>
                new EqualLeftTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                EqualTerm.Reduce(context, this.Argument0, rhs);
        }
    }

    public sealed class EqualTerm : BinaryOperatorTerm<EqualTerm>
    {
        internal EqualTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override Term HigherOrder =>
            BooleanTerm.Type;

        protected override Term Create(Term lhs, Term rhs) =>
            new EqualTerm(lhs, rhs);

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs) =>
            lhs.Reduce(context).Equals(rhs.Reduce(context)) ?
                BooleanTerm.True :
                BooleanTerm.False;

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);

        protected override Term Infer(InferContext context, Term lhs, Term rhs)
        {
            context.Unify(lhs.HigherOrder, rhs.HigherOrder);
            return new EqualTerm(lhs, rhs);
        }
    }
}
