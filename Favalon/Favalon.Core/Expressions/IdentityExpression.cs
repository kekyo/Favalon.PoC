namespace Favalon.Expressions
{
    public abstract class IdentityExpression : TermExpression
    {
        protected IdentityExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        public abstract string Name { get; }
    }
}
