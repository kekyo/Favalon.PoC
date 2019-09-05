using System;

namespace Favalon.Terms
{
    public abstract class LiteralTerm<TTerm, TValue> : Term
        where TTerm : LiteralTerm<TTerm, TValue>
        where TValue : IEquatable<TValue>
    {
        public readonly TValue Value;

        protected LiteralTerm(TValue value) =>
            this.Value = value;

        public override int GetHashCode() =>
            this.Value?.GetHashCode() ?? 0;

        public bool Equals(TTerm? rhs) =>
            rhs is TTerm term ? (this.Value?.Equals(term.Value) ?? false) : false;

        public override bool Equals(Term? rhs) =>
            this.Equals(rhs as TTerm);

        public override string ToString() =>
            $"{this.GetType().Name}: {this.Value}";
    }
}
