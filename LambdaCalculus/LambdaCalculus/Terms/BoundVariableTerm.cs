using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class BoundVariableTerm : IdentityTerm<BoundVariableTerm>
    {
        private BoundVariableTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new BoundVariableTerm(identity, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new BoundVariableTerm(this.Identity, higherOrder);
        }

        public static BoundVariableTerm Create(string identity, Term higherOrder) =>
            new BoundVariableTerm(identity, higherOrder);
    }
}
