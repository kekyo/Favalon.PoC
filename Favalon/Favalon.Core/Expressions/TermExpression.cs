namespace Favalon.Expressions
{
    public interface ITermExpression :
        IExpression
    {
    }

    public abstract class TermExpression<TTermExpression> :
        Expression<TTermExpression>, ITermExpression
        where TTermExpression : Expression<TTermExpression>, ITermExpression
    {
        protected TermExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }
    }
}
