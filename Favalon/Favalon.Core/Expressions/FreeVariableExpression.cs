using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public sealed class FreeVariableExpression : IdentityExpression,
        IEquatable<FreeVariableExpression>, IComparable<FreeVariableExpression>
    {
        public readonly long Index;

        internal FreeVariableExpression(long index) :
            base(UndefinedExpression.Instance) =>
            this.Index = index;

        public override string Name =>
            $"'{this.Index}";

        protected internal override string FormatReadableString(FormatStringContext context)
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

        public bool Equals(FreeVariableExpression other) =>
            this.Index.Equals(other.Index);

        public override bool Equals(IdentityExpression other) =>
            other is FreeVariableExpression freeVariable ? this.Equals(freeVariable) : false;

        public int CompareTo(FreeVariableExpression other) =>
            this.Index.CompareTo(other.Index);
    }
}
