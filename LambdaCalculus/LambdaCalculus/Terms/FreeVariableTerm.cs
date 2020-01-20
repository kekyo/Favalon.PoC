using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class FreeVariableTerm : IdentityTerm<FreeVariableTerm>
    {
        private FreeVariableTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                // Ignore repeating self references (will cause stack overflow)
                return bound is FreeVariableTerm ? bound : bound.Reduce(context);
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new FreeVariableTerm(this.Identity, higherOrder);
        }

        public static FreeVariableTerm Create(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);
    }
}
