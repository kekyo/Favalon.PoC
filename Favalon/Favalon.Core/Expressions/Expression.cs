using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public interface IExpression
    {
        IExpression HigherOrder { get; }
        TextRange TextRange { get; }

        string ReadableString { get; }
        string StrictReadableString { get; }
        XElement Xml { get; }
        XElement StrictXml { get; }
    }

    public abstract partial class Expression :
        IExpression
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
        IExpression IExpression.HigherOrder =>
            this.HigherOrder;

        internal virtual void SetHigherOrder(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public TextRange TextRange { get; }

        public virtual bool ShowInAnnotation => 
            true;

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

    public abstract class Expression<TExpression> : Expression
        where TExpression : Expression<TExpression>
    {
        protected Expression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        protected internal virtual TExpression VisitInferring(ExpressionEnvironment environment, InferContext context) =>
            (TExpression)this;
    }
}
