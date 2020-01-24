using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public sealed class BoundIdentityTerm : IdentityTerm<BoundIdentityTerm>
    {
        private BoundIdentityTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new BoundIdentityTerm(identity, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new BoundIdentityTerm(this.Identity, higherOrder);
        }

        public static BoundIdentityTerm Create(string identity, Term higherOrder) =>
            new BoundIdentityTerm(identity, higherOrder);
    }
}
