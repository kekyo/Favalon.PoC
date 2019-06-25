using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // 'a -> 'b
        public Expression Parameter { get; private set; }
        public Expression Expression { get; private set; }

        private LambdaExpression(Expression parameter, Expression expression, Expression higherOrder) :
            base(higherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal LambdaExpression(Expression parameter, Expression expression) :
            this(parameter, expression,
                new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance))
        {
        }

        public override bool ShowInAnnotation =>
            this.Parameter.ShowInAnnotation && this.Expression.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatStringContext context)
        {
            var arrow = context.FancySymbols ? "→" : "->";
            return (this.Parameter is LambdaExpression) ?
                $"({this.Parameter.GetReadableString(context)}) {arrow} {this.Expression.GetReadableString(context)}" :
                $"{this.Parameter.GetReadableString(context)} {arrow} {this.Expression.GetReadableString(context)}";
        }

        protected internal override Expression VisitInferring(ExpressionEnvironment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            // Force replacing with new free variable at the variable parameter.
            // Because the bind expression infers excepted from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            var parameter = (this.Parameter is VariableExpression variable) ?
                variable.CreateWithFreeVariableIfUndefined(scoped, context) :
                this.Parameter.VisitInferring(scoped, context);
            var expression = this.Expression.VisitInferring(scoped, context);

            return new LambdaExpression(parameter, expression);
        }

        protected internal override TraverseInferringResults TraverseInferring(System.Func<Expression, int, Expression> yc, int rank)
        {
            this.Parameter = yc(this.Parameter, rank);
            this.Expression = yc(this.Expression, rank);

            return TraverseInferringResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatStringContext context) =>
            new[] { this.Parameter.CreateXml(context), this.Expression.CreateXml(context) };
    }
}
