using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : IdentityExpression
    {
        internal VariableExpression(string name, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Name = name;

        internal VariableExpression(string name, TextRange textRange) :
            this(name, UndefinedExpression.Instance, textRange)
        { }

        public override string Name { get; }

        internal VariableExpression CreateWithFreeVariableIfUndefined(ExpressionEnvironment environment, InferContext context)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var freeVariableHigherOrder = environment.CreateFreeVariable(this.HigherOrder.TextRange);
                var variable = new VariableExpression(this.Name, freeVariableHigherOrder, this.TextRange);
                environment.Bind(this.Name, variable);

                return variable;
            }
            else
            {
                return new VariableExpression(this.Name, this.HigherOrder, this.TextRange);
            }
        }

        protected internal override Expression VisitInferring(ExpressionEnvironment environment, InferContext context)
        {
            if (environment.TryGetBoundExpression(this.Name, out var resolved))
            {
                return new VariableExpression(this.Name, resolved.HigherOrder, this.TextRange);
            }
            else
            {
                return this.CreateWithFreeVariableIfUndefined(environment, context);
            }
        }

        protected internal override TraverseInferringResults TraverseInferring(System.Func<Expression, int, Expression> yc, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name, TextRange.Unknown);
    }
}
