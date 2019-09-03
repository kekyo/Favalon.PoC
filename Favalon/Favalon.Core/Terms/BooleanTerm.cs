namespace Favalon.Terms
{
    public sealed class BooleanTerm : LiteralTerm<BooleanTerm, bool>
    {
        public BooleanTerm(bool value) :
            base(value)
        { }
    }
}
