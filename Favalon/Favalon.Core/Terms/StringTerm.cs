namespace Favalon.Terms
{
    public sealed class StringTerm : LiteralTerm<StringTerm, string>
    {
        public StringTerm(string value) :
            base(value)
        { }

        public override string ToString() =>
            $"{this.GetType().Name}: \"{this.Value}\"";
    }
}
