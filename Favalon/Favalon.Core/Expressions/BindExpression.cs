using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        public readonly VariableExpression Variable;
        public readonly Expression Expression;
        public readonly Expression Body;

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

            var bind = new BindExpression(variable, expression, body);
            context.RegisterFixupHigherOrder(bind);

            return bind;
        }
    }
}
