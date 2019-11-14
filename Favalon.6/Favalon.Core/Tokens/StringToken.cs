using System;

namespace Favalon.Tokens
{
    public sealed class StringToken : Token<StringToken>
    {
        public StringToken(string value) :
            base(value)
        { }

        public override string ToString() =>
            $"{this.GetType().Name}: \"{this.Value}\"";
    }
}
