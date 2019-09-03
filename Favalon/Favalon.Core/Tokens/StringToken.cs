using System;

namespace Favalon.Tokens
{
    public sealed class StringToken : Token
    {
        public StringToken(string value) :
            base(value)
        { }

        public override string ToString() =>
            $"{this.GetType().Name}: \"{this.Value}\"";
    }
}
