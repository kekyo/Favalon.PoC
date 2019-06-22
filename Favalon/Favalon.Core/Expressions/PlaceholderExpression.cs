using System;
using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression :
        Expression, IEquatable<PlaceholderExpression>, IComparable<PlaceholderExpression>
    {
        public int Rank { get; }
        public int Index { get; internal set; }

        internal PlaceholderExpression(int rank, int index) :
            base(UndefinedExpression.Instance)
        {
            this.Rank = rank;
            this.Index = index;
        }

        internal override bool CanProduceSafeReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation)
        {
            var rank = new string('\'', this.Rank);
            var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
            var suffixIndex = this.Index / ('z' - 'a' + 1);
            var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
            return $"{rank}{ch}{suffix}";
        }

        public override int GetHashCode() =>
            this.Rank ^ this.Index;

        public bool Equals(PlaceholderExpression other) =>
            (this.Rank == other.Rank) && (this.Index == other.Index);

        public int CompareTo(PlaceholderExpression other)
        {
            var r = this.Rank.CompareTo(other.Rank);
            if (r != 0)
            {
                return r;
            }
            return this.Index.CompareTo(other.Index);
        }
    }
}
