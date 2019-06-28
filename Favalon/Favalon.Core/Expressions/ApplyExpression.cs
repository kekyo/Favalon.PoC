using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        // f x
        private ApplyExpression(Expression function, Expression argument, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal ApplyExpression(Expression function, Expression argument, TextRange textRange) :
            this(function, argument, UndefinedExpression.Instance, textRange)
        {
        }

        public Expression Function { get; private set; }
        public Expression Argument { get; private set; }

        public override bool ShowInAnnotation =>
            this.Argument.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatContext context) =>
            (this.Argument is ApplyExpression) ?
                $"{this.Function.GetReadableString(context)} ({this.Argument.GetReadableString(context)})" :
                $"{this.Function.GetReadableString(context)} {this.Argument.GetReadableString(context)}";

        protected internal override Expression VisitInferring(
            Environment environment, InferContext context)
        {
            var function = this.Function.VisitInferring(environment, context);
            var argument = this.Argument.VisitInferring(environment, context);
            var higherOrder = this.VisitInferringHigherOrder(environment, context);

            var variableHigherOrder = new LambdaExpression(argument.HigherOrder, higherOrder, this.TextRange);
            context.UnifyExpression(function.HigherOrder, variableHigherOrder);

            return new ApplyExpression(function, argument, higherOrder, this.TextRange);
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
