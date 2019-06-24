using System;

namespace Favalon.Expressions
{
    public abstract class IdentityExpression : TermExpression,
        IEquatable<IdentityExpression>, IComparable<IdentityExpression>
    {
        protected IdentityExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        public abstract string Name { get; }

        protected internal override string FormatReadableString(bool withAnnotation) =>
            this.Name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(IdentityExpression other) =>
            this.Name.Equals(other.Name);

        public int CompareTo(IdentityExpression other) =>
            this.Name.CompareTo(other.Name);
    }
}
