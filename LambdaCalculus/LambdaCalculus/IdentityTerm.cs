﻿namespace Favalon
{
    public sealed class IdentityTerm : HigherOrderHoldTerm
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder) :
            base(higherOrder) =>
            this.Identity = identity;

        public override Term Infer(InferContext context)
        {
            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                return bound;
            }

            var higherOrder = this.HigherOrder.Infer(context);

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
                return bound.Reduce(context);
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new IdentityTerm(this.Identity, higherOrder);
        }

        public override bool Equals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public void Deconstruct(out string identity) =>
            identity = this.Identity;
    }
}
