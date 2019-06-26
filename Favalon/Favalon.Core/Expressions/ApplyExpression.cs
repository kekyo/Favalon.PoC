using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public interface IApplyExpression :
        IExpression
    {
    }

    public sealed class ApplyExpression<TFunctionExpression, TArgumentExpression> :
        Expression<ApplyExpression<TFunctionExpression, TArgumentExpression>>, IApplyExpression
        where TFunctionExpression : Expression<TFunctionExpression>
        where TArgumentExpression : Expression<TArgumentExpression>
    {
        // f x
        private ApplyExpression(TFunctionExpression function, TArgumentExpression argument, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal ApplyExpression(TFunctionExpression function, TArgumentExpression argument, TextRange textRange) :
            this(function, argument, UndefinedExpression.Instance, textRange)
        {
        }

        public TFunctionExpression Function { get; private set; }
        public TArgumentExpression Argument { get; private set; }

        public override bool ShowInAnnotation =>
            this.Argument.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatContext context) =>
            (this.Argument is IApplyExpression) ?
                $"{this.Function.GetReadableString(context)} ({this.Argument.GetReadableString(context)})" :
                $"{this.Function.GetReadableString(context)} {this.Argument.GetReadableString(context)}";

        protected internal override ApplyExpression<TFunctionExpression, TArgumentExpression> VisitInferring(
            ExpressionEnvironment environment, InferContext context)
        {
            var function = this.Function.VisitInferring(environment, context);
            var argument = this.Argument.VisitInferring(environment, context);

            var resultHigherOrder = environment.CreateFreeVariable(this.TextRange);

            var variableHigherOrder = new LambdaExpression<Expression, FreeVariableExpression>(argument.HigherOrder, resultHigherOrder, this.TextRange);
            context.UnifyExpression(function.HigherOrder, variableHigherOrder);

            return new ApplyExpression<TFunctionExpression, TArgumentExpression>(function, argument, resultHigherOrder, this.TextRange);
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank)
        {
            this.Function = context.FixupHigherOrders(this.Function, rank);
            this.Argument = context.FixupHigherOrders(this.Argument, rank);

            return TraverseInferringResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatContext context) =>
            new[] { this.Function.CreateXml(context), this.Argument.CreateXml(context) };
    }
}
