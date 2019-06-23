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

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(IdentityExpression other) =>
            this.Name.Equals(other.Name);

        public int CompareTo(IdentityExpression other) =>
            this.Name.CompareTo(other.Name);

        internal override bool CanProduceSafeReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Name;
    }
}
