using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public sealed class NewExpression : Expression
    {
        // * -> 'a
        private NewExpression(IdentityExpression argument, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Argument = argument;

        internal NewExpression(IdentityExpression argument, TextRange textRange) :
            this(argument, new LambdaExpression(UndefinedExpression.Instance, UndefinedExpression.Instance, textRange), textRange)
        {
        }

        public IdentityExpression Argument { get; private set; }

        public override bool ShowInAnnotation =>
            this.Argument.ShowInAnnotation;

        protected internal override string FormatReadableString(FormatContext context) =>
            $"new {this.Argument.FormatReadableString(context)}";

        protected internal override Expression VisitInferring(
            Environment environment, InferContext context)
        {
            // TODO: Correct real constructor argument type.
            var higherOrder = new LambdaExpression(
                environment.CreatePlaceholder(this.TextRange),
                environment.CreatePlaceholder(this.Argument, this.TextRange),
                this.TextRange);
            return new NewExpression(this.Argument, higherOrder, this.TextRange);
        }

        protected internal override TraverseInferringResults FixupHigherOrders(InferContext context, int rank)
        {
            this.Argument = context.FixupHigherOrders(this.Argument, rank);

            return TraverseInferringResults.RequeireHigherOrder;
        }

        protected internal override IEnumerable<XObject> CreateXmlChildren(FormatContext context) =>
            new[] { this.Argument.CreateXml(context) };
    }
}
