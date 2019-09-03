using System;

namespace Favalon.Tokens
{
    public abstract class Token : IEquatable<Token?>
    {
        public readonly string Value;

        public Token(string value) =>
            this.Value = value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Token? rhs) =>
            rhs is Token ? this.Value.Equals(rhs.Value) : false;

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as Token);

        public override string ToString() =>
            $"{this.GetType().Name}: {this.Value}";
    }
}
