using Favalon.Expressions.Internals;

using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Favalon.Core.Tests")]

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public Expression HigherOrder { get; internal set; }

        public virtual bool ShowInAnnotation => 
            true;

        protected internal virtual Expression Visit(Environment environment, InferContext context) =>
            this;

        protected internal virtual bool TraverseChildren(System.Func<Expression, int, Expression> ycon, int rank) =>
            false;

        public Expression Infer(Environment environment, int rank = 0)
        {
            var context = new InferContext();
            var visited = this.Visit(environment, context);
            var fixup = context.FixupHigherOrders(visited, rank);
            var aggregated = context.AggregatePlaceholders(fixup, rank);
            context.RearrangePlaceholderIndex();
            return aggregated;
        }

        protected abstract string FormatReadableString(bool withAnnotation);

        private string InternalFormatReadableString(bool withAnnotation) =>
            (this is TermExpression) ?
                this.FormatReadableString(withAnnotation) :
                $"({this.FormatReadableString(withAnnotation)})";

        public string GetReadableString(bool withAnnotation) =>
            (withAnnotation && this.HigherOrder.ShowInAnnotation) ?
                $"{this.InternalFormatReadableString(true)}:{this.HigherOrder.InternalFormatReadableString(false)}" :
                this.FormatReadableString(false);

        public string ReadableString =>
            this.FormatReadableString(false);

        public override string ToString() =>
            $"{this.GetType().Name.Replace("Expression", string.Empty)}: {this.GetReadableString(true)}";

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator Expression(string name) =>
            new VariableExpression(name);
    }
}
