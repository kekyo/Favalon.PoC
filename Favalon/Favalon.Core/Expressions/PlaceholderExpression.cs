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
            var higherOrder = VisitInferringHigherOrder(environment, this.HigherOrder, context);
            return new PlaceholderExpression(this.Index, higherOrder);
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
