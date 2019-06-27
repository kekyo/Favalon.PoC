using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public abstract class VariableExpression : IdentityExpression
    {
        private protected VariableExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;
    }
}
