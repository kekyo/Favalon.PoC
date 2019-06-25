using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract partial class Expression
    {
        protected internal enum TraverseInferringResults
        {
            Finished,
            RequeireHigherOrder
        }

        protected Expression(Expression higherOrder, TextRange textRange)
        {
            this.HigherOrder = higherOrder;
            this.TextRange = textRange;
        }

        public Expression HigherOrder { get; internal set; }

        public readonly TextRange TextRange;

        public virtual bool ShowInAnnotation => 
            true;

        protected internal virtual Expression VisitInferring(ExpressionEnvironment environment, InferContext context) =>
            this;

        protected internal virtual TraverseInferringResults TraverseInferring(System.Func<Expression, int, Expression> ycon, int rank) =>
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

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name, TextRange.Unknown);
    }
}
