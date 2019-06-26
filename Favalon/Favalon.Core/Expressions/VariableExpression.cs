using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public interface IVariableExpression :
        IIdentityExpression
    {
    }

    public abstract class VariableExpression : IdentityExpression, IVariableExpression
    {
        private protected VariableExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;
    }
}
