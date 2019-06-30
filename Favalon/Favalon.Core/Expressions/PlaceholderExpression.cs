using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : VariableExpression, IEquatable<PlaceholderExpression?>
    {
        public readonly int Index;

        internal PlaceholderExpression(int index, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Index = index;

        public override string Name
        {
            get
            {
                var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
                var suffixIndex = this.Index / ('z' - 'a' + 1);
                var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                return $"'{ch}{suffix}";
            }
        }

        protected override Expression VisitInferring(Environment environment, InferContext context)
        {
            if (environment.GetRelatedExpression(this) is TermExpression related)
            {
                var relatedHigherOrder = VisitInferringHigherOrder(environment, related.HigherOrder, context);
                var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
                Unify(environment, relatedHigherOrder, higherOrder);

                return new PlaceholderExpression(this.Index, relatedHigherOrder);
            }
            else
            {
                var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
                return new PlaceholderExpression(this.Index, higherOrder);
            }
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(Environment environment, InferContext context)
        {
            context.TouchedInResolving(this);

            return (environment.GetRelatedExpression(this) is TermExpression related) ?
                (true, related) : (false, this);
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderExpression? other) =>
            (other == null) ? false :
            object.ReferenceEquals(this, other) ? true :
            this.Index.Equals(other.Index);

        public override bool Equals(VariableExpression? other) =>
            this.Equals(other as PlaceholderExpression);

        public override bool Equals(object obj) =>
            this.Equals(obj as PlaceholderExpression);
    }
}
