namespace Favalon.Expressions
{
    public sealed class StringExpression : LiteralExpression<string>
    {
        public StringExpression(string value) :
            base(value)
        { }

        public override string ToString() =>
            $"\"{this.Value}\"";
    }
}
