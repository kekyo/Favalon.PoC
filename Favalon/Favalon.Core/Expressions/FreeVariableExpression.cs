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

        public FreeVariableExpression CloneWithPlaceholderIfUndefined(ExpressionEnvironment environment)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var placeholderHigherOrder = environment.CreatePlaceholder(this.HigherOrder.TextRange);

                // HACK: Environment.Bind can accept without same named free variable.
                //   NamedPlaceholderExpression is registered by owned variable.
                environment.Bind(
                    this.Name,
                    new NamedPlaceholderExpression(this.Name, placeholderHigherOrder, this.TextRange));

                return new FreeVariableExpression(this.Name, placeholderHigherOrder, this.TextRange);
            }
            else
            {
                return new FreeVariableExpression(this.Name, this.HigherOrder, this.TextRange);
            }
        }

        protected internal override Expression VisitInferring(
            ExpressionEnvironment environment, InferContext context)
        {
            if (environment.TryGetBoundExpression(this.Name, out var resolved))
            {
                return new FreeVariableExpression(this.Name, resolved.HigherOrder, this.TextRange);
            }
            else
            {
                return this.CloneWithPlaceholderIfUndefined(environment);
            }
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;
    }
}
