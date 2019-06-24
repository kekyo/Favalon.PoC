using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        protected internal enum TraverseResults
        {
            Finished,
            RequeireHigherOrder
        }

        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public Expression HigherOrder { get; internal set; }

        public virtual bool ShowInAnnotation => 
            true;

        protected internal virtual Expression Visit(Environment environment, InferContext context) =>
            this;

        protected internal virtual TraverseResults Traverse(System.Func<Expression, int, Expression> ycon, int rank) =>
            TraverseResults.Finished;

        protected internal abstract string FormatReadableString(FormatStringContext context);

        public string ReadableString =>
            this.FormatReadableString(new FormatStringContext(false, false, false));

        public string StrictReadableString =>
            this.FormatReadableString(new FormatStringContext(true, true, false));

        public override string ToString() =>
            $"{this.GetExpressionShortName()}: {this.GetReadableString(true, true, false)}";

        protected internal virtual IEnumerable<XObject> CreateXmlChildren(FormatStringContext context) =>
            Enumerable.Empty<XObject>();

        public XElement Xml =>
            this.CreateXml(new FormatStringContext(false, false, true));

        public XElement StrictXml =>
            this.CreateXml(new FormatStringContext(true, true, true));

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }
}
