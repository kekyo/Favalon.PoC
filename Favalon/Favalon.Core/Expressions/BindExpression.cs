using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

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

        public VariableExpression Variable { get; private set; }
        public Expression Expression { get; private set; }
        public Expression Body { get; private set; }

        protected internal override string FormatReadableString(FormatStringContext context) =>
            $"{this.Variable.GetReadableString(context)} = {this.Expression.GetReadableString(context)} in {this.Body.GetReadableString(context)}";

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            // Bind expression scope details:
            // let x = y in z
            //     |   |    |
            //     | outer  |
            //     |        |
            //     +-inner--+

            var scoped = environment.NewScope();
            var expression = this.Expression.Visit(scoped, context);

            // Force replacing with new free variable.
            // Because the bind expression excepts inferring from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            //var variable = (VariableExpression)this.Variable.Visit(scoped, context);
            var variable = this.Variable.CreateWithFreeVariableIfUndefined(scoped, context);

            context.UnifyExpression(variable.HigherOrder, expression.HigherOrder);
            scoped.SetNamedExpression(variable.Name, expression);

            var body = this.Body.Visit(scoped, context);

            return new BindExpression(variable, expression, body);
        }

        protected internal override TraverseResults Traverse(System.Func<Expression, int, Expression> yc, int rank)
        {
            this.Variable = (VariableExpression)yc(this.Variable, rank);
            this.Expression = yc(this.Expression, rank);
            this.Body = yc(this.Body, rank);

            return TraverseResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatStringContext context) =>
            new[] { this.Variable.CreateXml(context), this.Expression.CreateXml(context), this.Body.CreateXml(context) };
    }
}
