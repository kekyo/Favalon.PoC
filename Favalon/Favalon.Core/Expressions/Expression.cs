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

        protected internal abstract string FormatReadableString(bool withAnnotation);

        public string ReadableString =>
            this.FormatReadableString(false);

        public override string ToString() =>
            $"{this.GetExpressionShortName()}: {this.GetReadableString(true)}";

        protected internal virtual IEnumerable<XObject> CreateXmlChildren(bool strictAnnotation) =>
            Enumerable.Empty<XObject>();

        public XElement Xml =>
            this.CreateXml(false);

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }
}
