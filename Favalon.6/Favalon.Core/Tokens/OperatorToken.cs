using System;

namespace Favalon.Tokens
{
    public sealed class OperatorToken :
        Token<OperatorToken>, IVariableToken
    {
        public OperatorToken(string value) :
            base(value)
        { }

        string IVariableToken.Value => base.Value;
    }
}
