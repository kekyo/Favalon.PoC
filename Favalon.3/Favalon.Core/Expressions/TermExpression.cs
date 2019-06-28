namespace Favalon.Expressions
{
    public abstract class TermExpression : Expression
    {
        protected TermExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }
    }
}
