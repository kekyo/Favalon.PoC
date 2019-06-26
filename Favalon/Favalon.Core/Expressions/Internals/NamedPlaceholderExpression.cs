namespace Favalon.Expressions.Internals
{
    internal sealed class NamedPlaceholderExpression : IdentityExpression
    {
        internal NamedPlaceholderExpression(string name, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Name = name;

        public override string Name { get; }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;
    }
}
