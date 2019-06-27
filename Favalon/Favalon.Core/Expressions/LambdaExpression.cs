using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // 'a -> 'b
        internal LambdaExpression(Expression parameter, Expression expression, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal LambdaExpression(Expression parameter, Expression expression, TextRange textRange) :
            this(parameter, expression, new LambdaExpression(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance, textRange), textRange)
        {
        }

        public Expression Parameter { get; private set; }
        public Expression Expression { get; private set; }

        public override bool ShowInAnnotation =>
            this.Parameter.ShowInAnnotation && this.Expression.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatContext context)
        {
            var arrow = context.FancySymbols ? "→" : "->";
            return (this.Parameter is LambdaExpression) ?
                $"({this.Parameter.GetReadableString(context)}) {arrow} {this.Expression.GetReadableString(context)}" :
                $"{this.Parameter.GetReadableString(context)} {arrow} {this.Expression.GetReadableString(context)}";
        }

        protected internal override Expression VisitInferring(
            Environment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            // Force replacing with new free variable at the variable parameter.
            // Because the bind expression infers excepted from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            var parameter = (this.Parameter is FreeVariableExpression freeVariable) ?
                scoped.Register(freeVariable) :
                this.Parameter.VisitInferring(scoped, context);
            var expression = this.Expression.VisitInferring(scoped, context);

            return new LambdaExpression(parameter, expression, this.TextRange);
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank)
        {
            this.Parameter = context.FixupHigherOrders(this.Parameter, rank);
            this.Expression = context.FixupHigherOrders(this.Expression, rank);

            return TraverseInferringResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatContext context) =>
            new[] { this.Parameter.CreateXml(context), this.Expression.CreateXml(context) };
    }
}
