namespace Favalon.Terms
{
    public sealed class FreeVariableTerm : IdentityTerm<FreeVariableTerm>
    {
        private FreeVariableTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);

        public static FreeVariableTerm Create(string identity, Term higherOrder) =>
            new FreeVariableTerm(identity, higherOrder);
    }
}
