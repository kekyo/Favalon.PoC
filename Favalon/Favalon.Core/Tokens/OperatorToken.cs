using System;

namespace Favalon.Tokens
{
    public sealed class OperatorToken : VariableToken
    {
        public OperatorToken(string value) :
            base(value)
        { }
    }
}
