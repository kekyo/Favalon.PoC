using System;

namespace Favalon.Tokens
{
    public sealed class SeparatorToken :
        Token, IEquatable<SeparatorToken?>
    {
        private SeparatorToken()
        { }

        public override int GetHashCode() =>
            0;

        public bool Equals(SeparatorToken? other) =>
            other != null;

        public override bool Equals(object obj) =>
            this.Equals(obj as SeparatorToken);

        public override string ToString() =>
            string.Empty;

        internal static readonly SeparatorToken Instance =
            new SeparatorToken();
    }
}
