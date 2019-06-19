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

        internal override string GetInternalReadableString(bool withAnnotation)
        {
            var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
            var suffixIndex = this.Index / ('z' - 'a' + 1);
            var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
            return $"'{ch}{suffix}";
        }

        internal override Expression Visit(ExpressionEnvironment environment) =>
            this;

        internal override void Resolve(ExpressionEnvironment environment)
        {
        }

        public bool Equals(PlaceholderExpression other) =>
            this.Index == other.Index;
    }
}
