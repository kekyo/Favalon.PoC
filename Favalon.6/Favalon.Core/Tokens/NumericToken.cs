using System;

namespace Favalon.Tokens
{
    public sealed class NumericToken : Token<NumericToken>
    {
        public NumericToken(string value) :
            base(value)
        { }
    }
}
