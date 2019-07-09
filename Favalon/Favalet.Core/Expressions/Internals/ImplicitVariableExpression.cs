namespace Favalet.Expressions.Internals
{
    public sealed class ImplicitVariableExpression : SymbolicVariableExpression
    {
        private ImplicitVariableExpression(string name, Expression higherOrder) :
            base(name, higherOrder)
        { }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferringImplicitVariable(
                environment,
                (name, higherOrder) => new ImplicitVariableExpression(name, higherOrder),
                higherOrderHint);

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var higherOrder = environment.Visit(this.HigherOrder);
            return new ImplicitVariableExpression(this.Name, higherOrder);
        }

        public static ImplicitVariableExpression Create(string name, Expression higherOrder) =>
            new ImplicitVariableExpression(name, higherOrder);
    }
}
