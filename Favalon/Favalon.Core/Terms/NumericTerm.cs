namespace Favalon.Terms
{
    public sealed class NumericTerm : LiteralTerm<NumericTerm, string>
    {
        public NumericTerm(string value) :
            base(value)
        { }
    }
}
