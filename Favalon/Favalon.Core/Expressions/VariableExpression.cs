using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : IdentityExpression
    {
        internal VariableExpression(string name) :
            base(UndefinedExpression.Instance) =>
            this.Name = name;

        private VariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string Name { get; }

        internal override bool CanProduceSafeReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Name;

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            if (environment.TryGetNamedExpression(this.Name, out var resolved))
            {
                return new VariableExpression(this.Name, resolved.HigherOrder);
            }

            var placeholder = context.CreatePlaceholder(1);
            var variable = new VariableExpression(this.Name, placeholder);
            environment.SetNamedExpression(this.Name, variable);

            return variable;
        }

        protected internal override bool TraverseChildren(System.Func<Expression, Expression> yc) =>
            true;

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name);
    }
}
