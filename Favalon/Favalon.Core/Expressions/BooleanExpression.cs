namespace Favalon.Expressions
{
    public sealed class BooleanExpression : LiteralExpression<bool>
    {
        public BooleanExpression(bool value) :
            base(value)
        { }
    }
}
