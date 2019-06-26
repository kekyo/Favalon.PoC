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

        internal VariableExpression CreateWithFreeVariableIfUndefined(ExpressionEnvironment environment)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var freeVariableHigherOrder = environment.CreateFreeVariable(this.HigherOrder.TextRange);
                return new VariableExpression(this.Name, freeVariableHigherOrder, this.TextRange);
            }
            else
            {
                return new VariableExpression(this.Name, this.HigherOrder, this.TextRange);
            }
        }

        protected internal override Expression VisitInferring(ExpressionEnvironment environment, InferContext context)
        {
            if (environment.GetBoundExpression(this.Name) is (VariableExpression b, Expression e))
            {
                var bound = b.CreateWithFreeVariableIfUndefined(environment);
                var expression = e.VisitInferring(environment, context);

                context.UnifyExpression(bound.HigherOrder, expression.HigherOrder);
                return bound;
            }
            else
            {
                var bound = this.CreateWithFreeVariableIfUndefined(environment);
                return bound;
            }
        }

        protected internal override TraverseInferringResults TraverseInferring(System.Func<Expression, int, Expression> yc, int rank) =>
            TraverseInferringResults.RequeireHigherOrder;

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name, TextRange.Unknown);
    }
}
