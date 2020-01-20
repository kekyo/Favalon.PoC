using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class IdentityTerm : HigherOrderHoldTerm
    {
        public readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder) :
            base(higherOrder) =>
            this.Identity = identity;

        public override Term Infer(InferContext context)
        {
            var higherOrder = this.HigherOrder.Infer(context);

            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                context.Unify(bound.HigherOrder, higherOrder);

                return new IdentityTerm(this.Identity, higherOrder);
            }

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new IdentityTerm(this.Identity, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new IdentityTerm(this.Identity, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                // Ignore repeating self references (will cause stack overflow)
                return bound is IdentityTerm ? bound : bound.Reduce(context);
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new IdentityTerm(this.Identity, higherOrder);
        }

        protected override bool OnEquals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public void Deconstruct(out string identity) =>
            identity = this.Identity;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Identity;
    }
}
