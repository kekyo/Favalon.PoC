namespace LambdaCalculus
{
    public sealed class IdentityTerm : Term
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(ReduceContext context) =>
            context.GetBoundTerm(this.Identity) is Term bound ?
                bound.Reduce(context) :
                new IdentityTerm(this.Identity, this.HigherOrder.Reduce(context));

        public override Term Infer(InferContext context) =>
            context.GetBoundTerm(this.Identity) is Term bound ?
                bound :
                new IdentityTerm(this.Identity, this.HigherOrder.Infer(context));

        public override Term Fixup(InferContext context) =>
            new IdentityTerm(this.Identity, this.HigherOrder.Fixup(context));

        public override bool Equals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;

        public override int GetHashCode() =>
            this.Identity.GetHashCode();
    }
}
