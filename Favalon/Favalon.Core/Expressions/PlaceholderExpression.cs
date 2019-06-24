using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : IdentityExpression,
        IEquatable<PlaceholderExpression>, IComparable<PlaceholderExpression>
    {
        public readonly long Index;

        internal PlaceholderExpression(long index) :
            base(UndefinedExpression.Instance) =>
            this.Index = index;

        public override string Name =>
            $"'T{this.Index}";

        protected internal override string FormatReadableString(FormatStringContext context)
        {
            if (context.StrictPlaceholderNaming)
            {
                return this.Name;
            }
            else
            {
                var adjustedIndex = context.GetAdjustedIndex(this);

                var ch = (char)('a' + (adjustedIndex % ('z' - 'a' + 1)));
                var suffixIndex = adjustedIndex / ('z' - 'a' + 1);
                var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                return $"'{ch}{suffix}";
            }
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderExpression other) =>
            this.Index.Equals(other.Index);

        public override bool Equals(IdentityExpression other) =>
            other is PlaceholderExpression placeholder ? this.Equals(placeholder) : false;

        public int CompareTo(PlaceholderExpression other) =>
            this.Index.CompareTo(other.Index);
    }
}
