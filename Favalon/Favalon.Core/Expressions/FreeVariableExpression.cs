using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class FreeVariableExpression : VariableExpression
    {
        internal FreeVariableExpression(string name, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Name = name;

        internal FreeVariableExpression(string name, TextRange textRange) :
            this(name, UndefinedExpression.Instance, textRange)
        { }

        public override string Name { get; }

        internal FreeVariableExpression CloneWithPlaceholderIfUndefined(Environment environment)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var placeholderHigherOrder = environment.CreatePlaceholder(this.HigherOrder.TextRange);
                return new FreeVariableExpression(this.Name, placeholderHigherOrder, this.TextRange);
            }
            else
            {
                return new FreeVariableExpression(this.Name, this.HigherOrder, this.TextRange);
            }
        }

        protected internal override Expression VisitInferring(
            Environment environment, InferContext context)
        {
            if (environment.TryGetBoundExpression(this, out var resolved))
            {
                context.UnifyExpression(this.HigherOrder, resolved.HigherOrder);
                return new FreeVariableExpression(this.Name, resolved.HigherOrder, this.TextRange);
            }
            else
            {
                throw new System.ArgumentException(this.ReadableString);
            }
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;
    }
}
