namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        private Expression higherOrder =
            UnspecifiedExpression.Instance;
        
        public readonly Expression Function;
        public readonly Expression Argument;

        public ApplyExpression(Expression function, Expression argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Expression HigherOrder =>
            higherOrder;

        public override bool Equals(Expression? rhs) =>
            rhs is ApplyExpression apply ?
                (this.Function.Equals(apply.Function) && this.Argument.Equals(apply.Argument)) :
                false;

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override string ToString() =>
            $"{this.Function} {this.Argument}";
    }
}
