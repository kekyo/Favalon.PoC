namespace Favalon.Terms
{
    public sealed class BoundIdentityTerm : IdentityTerm<BoundIdentityTerm>
    {
        private BoundIdentityTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new BoundIdentityTerm(identity, higherOrder);

        public static BoundIdentityTerm Create(string identity, Term higherOrder) =>
            new BoundIdentityTerm(identity, higherOrder);
    }
}
