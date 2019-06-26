using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public interface ILambdaExpression :
        IExpression
    {
        Expression Parameter { get; }
        Expression Expression { get; }
    }

    public sealed class LambdaExpression<TParameterExpression, TExpressionExpression> :
        Expression<LambdaExpression<TParameterExpression, TExpressionExpression>>, ILambdaExpression
        where TParameterExpression : Expression<TParameterExpression>, IVariableExpression
        where TExpressionExpression : Expression<TExpressionExpression>
    {
        // 'a -> 'b
        private LambdaExpression(TParameterExpression parameter, TExpressionExpression expression, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        internal LambdaExpression(TParameterExpression parameter, TExpressionExpression expression, TextRange textRange) :
            this(parameter, expression, new LambdaExpression<Expression, Expression>(parameter.HigherOrder, expression.HigherOrder, KindExpression.Instance, textRange), textRange)
        {
        }

        public TParameterExpression Parameter { get; private set; }
        public TExpressionExpression Expression { get; private set; }

        Expression ILambdaExpression.Parameter =>
            this.Parameter;
        Expression ILambdaExpression.Expression =>
            this.Expression;

        public override bool ShowInAnnotation =>
            this.Parameter.ShowInAnnotation && this.Expression.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatContext context)
        {
            var arrow = context.FancySymbols ? "→" : "->";
            return (this.Parameter is ILambdaExpression) ?
                $"({this.Parameter.GetReadableString(context)}) {arrow} {this.Expression.GetReadableString(context)}" :
                $"{this.Parameter.GetReadableString(context)} {arrow} {this.Expression.GetReadableString(context)}";
        }

        protected internal override LambdaExpression<TParameterExpression, TExpressionExpression> VisitInferring(
            ExpressionEnvironment environment, InferContext context)
        {
            var scoped = environment.NewScope();

            // Force replacing with new free variable at the variable parameter.
            // Because the bind expression infers excepted from derived environments,
            // but uses variable expression instead simple name string.
            // It requires annotation processing.
            var parameter = (this.Parameter is VariableExpression variable) ?
                (TParameterExpression)variable.CreateWithFreeVariableIfUndefined(scoped) :
                this.Parameter.VisitInferring(scoped, context);
            var expression = this.Expression.VisitInferring(scoped, context);

            return new LambdaExpression<TParameterExpression, TExpressionExpression>(parameter, expression, this.TextRange);
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
