using Favalet.Expressions.Internals;

namespace Favalet.Expressions.Specialized
{
    public sealed class KindExpression : SymbolicVariableExpression
    {
        private KindExpression(string kindName, Expression higherOrder) :
            base(kindName, higherOrder)
        { }

        public override bool IsAlwaysVisibleInAnnotation =>
            false;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Kind)" : "*";

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);
            var variable = new KindExpression(this.Name, higherOrder);
            environment.Memoize(this, variable);
            return variable;
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        internal static readonly KindExpression Instance =
            new KindExpression("*", null!);
    }
}
