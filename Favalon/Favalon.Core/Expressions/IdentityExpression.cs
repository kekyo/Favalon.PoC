using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public interface IIdentityExpression :
        ITermExpression, IEquatable<IIdentityExpression>
    {
        string Name { get; }
    }

    public abstract class IdentityExpression<TIdentityExpression> :
        TermExpression<TIdentityExpression>, IIdentityExpression
        where TIdentityExpression : TermExpression<TIdentityExpression>, IIdentityExpression
    {
        protected IdentityExpression(Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        { }

        public abstract string Name { get; }

        protected internal override string FormatReadableString(FormatContext context) =>
            this.Name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public virtual bool Equals(IIdentityExpression other) =>
            this.Name.Equals(other.Name);

        bool IEquatable<IIdentityExpression>.Equals(IIdentityExpression other) =>
            this.Equals(other);

        public override bool Equals(object obj) =>
            obj is IIdentityExpression identity ? this.Equals(identity) : false;
    }
}
