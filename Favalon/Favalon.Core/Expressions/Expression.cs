using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Favalon.Expressions
{
    public abstract partial class Expression
    {
        protected internal enum TraverseInferringResults
        {
            Finished,
            RequeireHigherOrder
        }

        private protected Expression(Expression higherOrder, TextRange textRange)
        {
            this.HigherOrder = higherOrder;
            this.TextRange = textRange;
        }

        public Expression HigherOrder { get; private set; }

        internal virtual void SetHigherOrder(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public TextRange TextRange { get; }

        public virtual bool ShowInAnnotation => 
            true;

        protected internal Expression VisitInferringHigherOrder(Environment environment, InferContext context)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                return environment.CreatePlaceholder(this.TextRange);
            }
            else
            {
                var visited = this.HigherOrder.VisitInferring(environment, context);
                context.UnifyExpression(this.HigherOrder, visited);
                return visited;
            }
        }

        protected internal virtual Expression VisitInferring(Environment environment, InferContext context) =>
            this;
        protected internal virtual TraverseInferringResults FixupHigherOrders(InferContext context, int rank) =>
            TraverseInferringResults.Finished;

        protected internal abstract string FormatReadableString(FormatContext context);

        public string ReadableString =>
            this.FormatReadableString(new FormatContext(false, false, false));

        public string StrictReadableString =>
            this.FormatReadableString(new FormatContext(true, true, false));

        public override string ToString() =>
            $"{this.GetExpressionShortName()}: {this.GetReadableString(true, true, false)}";

        protected internal virtual IEnumerable<XObject> CreateXmlChildren(FormatContext context) =>
            Enumerable.Empty<XObject>();

        public XElement Xml =>
            this.CreateXml(new FormatContext(false, false, true));

        public XElement StrictXml =>
            this.CreateXml(new FormatContext(true, true, true));
    }
}
