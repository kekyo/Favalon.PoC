using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public sealed class FreeVariableTerm : IdentityTerm<FreeVariableTerm>
    {
        private FreeVariableTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);

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

        public override Term Reduce(ReduceContext context)
        {
            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                // Ignore repeating self references (will cause stack overflow)
                return bound is IIdentityTerm ? bound : bound.Reduce(context);
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(this.Identity, higherOrder);
        }

        public static FreeVariableTerm Create(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);
    }
}
