using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        public Expression Variable { get; private set; }
        public Expression Expression { get; private set; }
        public Expression Body { get; private set; }

        internal BindExpression(VariableExpression variable, Expression expression, Expression body) :
            base(body.HigherOrder)
        {
            this.Variable = variable;
            this.Expression = expression;
            this.Body = body;
        }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"{this.Variable.GetReadableString(withAnnotation)} = {this.Expression.GetReadableString(withAnnotation)} in {this.Body.GetReadableString(withAnnotation)}";

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            var variable = (VariableExpression)this.Variable.Visit(scoped, context);
            var expression = this.Expression.Visit(scoped, context);

            context.UnifyExpression(variable.HigherOrder, expression.HigherOrder);
            scoped.SetNamedExpression(variable.Name, expression);

            var body = this.Body.Visit(scoped, context);

            return new BindExpression(variable, expression, body);
        }

        internal override Expression FixupChildren(InferContext context)
        {
            this.Variable = context.Fixup(this.Variable);
            this.Expression = context.Fixup(this.Expression);
            this.Body = context.Fixup(this.Body);
            this.HigherOrder = context.Fixup(this.HigherOrder);
            return this;
        }
    }
}
