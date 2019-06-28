using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression :
        VariableExpression, IEquatable<PlaceholderExpression>, IComparable<PlaceholderExpression>
    {
        public readonly long Index;

        internal PlaceholderExpression(long index, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Index = index;

        public override string Name =>
            $"'{this.Index}";

        protected internal override string FormatReadableString(FormatContext context)
        {
            if (context.StrictNaming)
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

        bool IEquatable<PlaceholderExpression>.Equals(PlaceholderExpression other) =>
            this.Equals(other);

        public override bool Equals(IdentityExpression other) =>
            other is PlaceholderExpression placeholder ? this.Equals(placeholder) : false;

        public int CompareTo(PlaceholderExpression other) =>
            this.Index.CompareTo(other.Index);

        int IComparable<PlaceholderExpression>.CompareTo(PlaceholderExpression other) =>
            this.CompareTo(other);
    }
}
