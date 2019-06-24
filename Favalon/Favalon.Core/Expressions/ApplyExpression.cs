using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        // f x
        private ApplyExpression(Expression function, Expression argument, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal ApplyExpression(Expression function, Expression argument) :
            base((function.HigherOrder as LambdaExpression)?.Expression ?? function.HigherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public Expression Function { get; private set; }
        public Expression Argument { get; private set; }

        public override bool ShowInAnnotation =>
            this.Argument.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatStringContext context) =>
            (this.Argument is ApplyExpression) ?
                $"{this.Function.GetReadableString(context)} ({this.Argument.GetReadableString(context)})" :
                $"{this.Function.GetReadableString(context)} {this.Argument.GetReadableString(context)}";

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            var function = this.Function.Visit(environment, context);
            var argument = this.Argument.Visit(environment, context);

            var resultHigherOrder = environment.CreatePlaceholder();

            var variableHigherOrder = new LambdaExpression(argument.HigherOrder, resultHigherOrder);
            context.UnifyExpression(function.HigherOrder, variableHigherOrder);

            return new ApplyExpression(function, argument, resultHigherOrder);
        }

        protected internal override TraverseResults Traverse(System.Func<Expression, int, Expression> yc, int rank)
        {
            this.Function = yc(this.Function, rank);
            this.Argument = yc(this.Argument, rank);

            return TraverseResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatStringContext context) =>
            new[] { this.Function.CreateXml(context), this.Argument.CreateXml(context) };
    }
}
