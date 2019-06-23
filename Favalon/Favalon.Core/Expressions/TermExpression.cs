namespace Favalon.Expressions
{
    public abstract class TermExpression : Expression
    {
        protected TermExpression(Expression higherOrder) :
            base(higherOrder)
        { }
    }
}
