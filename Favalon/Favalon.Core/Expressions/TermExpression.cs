namespace Favalon.Expressions
{
    public interface ITermExpression :
        IExpression
    {
    }

    public abstract class TermExpression : Expression, ITermExpression
    {
        protected TermExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }
    }
}
