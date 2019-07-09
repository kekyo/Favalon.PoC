namespace Favalet.Expressions.Specialized
{
    public sealed class TypeExpression : SymbolicVariableExpression
    {
        private TypeExpression(string typeName, Expression higherOrder) :
            base(typeName, higherOrder)
        { }

        public override bool IsAlwaysVisibleInAnnotation =>
            this.Name != "%";

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Type)" : base.FormatReadableString(context);

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);
            var variable = new TypeExpression(this.Name, higherOrder);
            environment.Memoize(this, variable);
            return variable;
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        public static TypeExpression Create(string typeName) =>
            new TypeExpression(typeName, KindExpression.Instance);

        public static readonly TypeExpression Instance =
            new TypeExpression("%", KindExpression.Instance);
    }
}
