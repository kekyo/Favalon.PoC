namespace Favalon.Terms.Logical
{
    public sealed class ClrTrueTerm : TrueTerm
    {
        internal ClrTrueTerm() :
            base(ConstantTerm.ClrBooleanType)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            this;
    }

    public sealed class ClrFalseTerm : FalseTerm
    {
        internal ClrFalseTerm() :
            base(ConstantTerm.ClrBooleanType)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            this;
    }
}
