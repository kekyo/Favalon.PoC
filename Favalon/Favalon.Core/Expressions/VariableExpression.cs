using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : IdentityExpression
    {
        internal VariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        internal VariableExpression(string name) :
            base(UndefinedExpression.Instance) =>
            this.Name = name;

        public override string Name { get; }

        internal VariableExpression CreateWithFreeVariableIfUndefined(ExpressionEnvironment environment, InferContext context)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var freeVariableHigherOrder = environment.CreateFreeVariable();
                var variable = new VariableExpression(this.Name, freeVariableHigherOrder);
                environment.Bind(this.Name, variable, false);

                return variable;
            }
            else
            {
                return new VariableExpression(this.Name, this.HigherOrder);
            }
        }

        protected internal override Expression VisitInferring(ExpressionEnvironment environment, InferContext context)
        {
            if (environment.TryGetBoundExpression(this.Name, out var resolved))
            {
                return new VariableExpression(this.Name, resolved.HigherOrder);
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
            new VariableExpression(name);
    }
}
