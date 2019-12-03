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
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Reduce(context) :
                new IdentityTerm(this.Identity, this.HigherOrder.Reduce(context));

        public override Term Infer(InferContext context)
        {
            if (context.GetBoundTerm(this.Identity) is Term bound)
            {
                return bound;
            }

            var higherOrder = this.HigherOrder.Infer(context);
            if (higherOrder is UnspecifiedTerm)
            {
                return new IdentityTerm(this.Identity, context.CreatePlaceholder(UnspecifiedTerm.Instance));
            }
            else
            {
                return new IdentityTerm(this.Identity, higherOrder);
            }
        }

        public override Term Fixup(InferContext context) =>
            new IdentityTerm(this.Identity, this.HigherOrder.Fixup(context));

        public override bool Equals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;
    }
}
