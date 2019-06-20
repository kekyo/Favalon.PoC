using System;

using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : Expression, IEquatable<PlaceholderExpression>
    {
        public readonly int Index;

        internal PlaceholderExpression(int index) :
            base(UndefinedExpression.Instance) =>
            this.Index = index;

        internal override bool CanProduceSafeReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation)
        {
            var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
            var suffixIndex = this.Index / ('z' - 'a' + 1);
            var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
            return $"'{ch}{suffix}";
        }

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context) =>
            this;

        public bool Equals(PlaceholderExpression other) =>
            this.Index == other.Index;
    }
}
