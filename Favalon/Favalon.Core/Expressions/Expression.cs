using Favalon.Expressions.Internals;

using System.Runtime.CompilerServices;

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
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.GetReadableString(true)}";

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }
}
