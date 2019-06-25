using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public abstract class IdentityExpression : TermExpression,
        IEquatable<IdentityExpression>
    {
        protected IdentityExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        public abstract string Name { get; }

        protected internal override string FormatReadableString(FormatStringContext context) =>
            this.Name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public virtual bool Equals(IdentityExpression other) =>
            this.Name.Equals(other.Name);

        public override bool Equals(object obj) =>
            obj is IdentityExpression identity ? this.Equals(identity) : false;

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator IdentityExpression(string name) =>
            new VariableExpression(name, TextRange.Unknown);
    }
}
