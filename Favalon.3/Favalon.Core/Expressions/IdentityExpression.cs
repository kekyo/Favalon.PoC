using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public abstract class IdentityExpression : TermExpression, IEquatable<IdentityExpression>
    {
        protected IdentityExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        public abstract string Name { get; }

        protected internal override string FormatReadableString(FormatContext context) =>
            this.Name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public virtual bool Equals(IdentityExpression other) =>
            this.Name.Equals(other.Name);

        bool IEquatable<IdentityExpression>.Equals(IdentityExpression other) =>
            this.Equals(other);

        public override bool Equals(object obj) =>
            obj is IdentityExpression identity ? this.Equals(identity) : false;
    }
}
