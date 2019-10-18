using System;

namespace Favalon.Tokens
{
    public abstract class Token
    {
        public readonly string Value;

        protected Token(string value) =>
            this.Value = value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override string ToString() =>
            $"{this.GetType().Name}: {this.Value}";
    }

    public abstract class Token<TToken> :
        Token, IEquatable<TToken?>
        where TToken : Token<TToken>
    {
        protected Token(string value) :
            base(value)
        { }

        public bool Equals(TToken? rhs) =>
            rhs is TToken ? this.Value.Equals(rhs.Value) : false;

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as TToken);

        public override string ToString() =>
            $"{this.GetType().Name}: {this.Value}";
    }
}
