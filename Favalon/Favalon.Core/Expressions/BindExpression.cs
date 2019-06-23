using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        internal BindExpression(VariableExpression variable, Expression expression, Expression body) :
            base(body.HigherOrder)
        {
            this.Variable = variable;
            this.Expression = expression;
            this.Body = body;
        }

        public Expression Variable { get; private set; }
        public Expression Expression { get; private set; }
        public Expression Body { get; private set; }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"{this.Variable.GetReadableString(withAnnotation)} = {this.Expression.GetReadableString(withAnnotation)} in {this.Body.GetReadableString(withAnnotation)}";

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            var variable = (VariableExpression)this.Variable.Visit(scoped, context);
            var expression = this.Expression.Visit(scoped, context);

            context.UnifyExpression(variable.HigherOrder, expression.HigherOrder);
            scoped.SetNamedExpression(variable.Name, expression);

            var body = this.Body.Visit(scoped, context);

            return new BindExpression(variable, expression, body);
        }

        protected internal override bool TraverseChildren(System.Func<Expression, int, Expression> yc, int rank)
        {
            this.Variable = yc(this.Variable, rank);
            this.Expression = yc(this.Expression, rank);
            this.Body = yc(this.Body, rank);

            return true;
        }
    }
}
