using System;

namespace Favalon.Tokens
{
    public sealed class BeginBracketToken :
        Token, IEquatable<BeginBracketToken?>
    {
        private BeginBracketToken()
        { }

        public bool Equals(BeginBracketToken? other) =>
            other != null;

        public override bool Equals(object obj) =>
            this.Equals(obj as BeginBracketToken);

        public override string ToString() =>
            "(";

        internal static readonly BeginBracketToken Instance =
            new BeginBracketToken();
    }
}
