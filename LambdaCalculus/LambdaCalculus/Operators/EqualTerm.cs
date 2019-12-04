namespace LambdaCalculus.Operators
{
    public sealed class EqualTerm : BinaryOperatorTerm<EqualTerm>
    {
        internal EqualTerm(Term lhs, Term rhs) :
            base(lhs, rhs)
        { }

        public override Term HigherOrder =>
            BooleanTerm.Type;

        protected override Term Create(Term lhs, Term rhs) =>
            new EqualTerm(lhs, rhs);

        public override Term Reduce(ReduceContext context) =>
            this.Lhs.Reduce(context).Equals(this.Rhs.Reduce(context)) ?
                BooleanTerm.True :
                BooleanTerm.False;

        protected override void Infer(InferContext context, Term lhs, Term rhs) =>
            context.Unify(lhs.HigherOrder, rhs.HigherOrder);
    }
}
