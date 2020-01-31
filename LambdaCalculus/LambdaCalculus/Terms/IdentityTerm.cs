using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public interface IIdentityTerm
    {
        string Identity { get; }
    }

    public abstract class IdentityTerm : Term, IIdentityTerm
    {
        internal IdentityTerm(string identity, Term higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public string Identity { get; }

        protected abstract Term OnCreate(string identity, Term higherOrder);

        public override Term Infer(InferContext context)
        {
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                context.Unify(bound.HigherOrder, higherOrder);

                return this.OnCreate(this.Identity, higherOrder);
            }

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(this.Identity, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(this.Identity, higherOrder);
        }

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public void Deconstruct(out string identity) =>
            identity = this.Identity;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Identity;
    }

    public abstract class IdentityTerm<T> : IdentityTerm
        where T : IdentityTerm
    {
        protected IdentityTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T rhs ? (this.Identity == rhs.Identity) : false;
    }
}
