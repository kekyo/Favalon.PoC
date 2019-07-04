using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    public struct FormattedString
    {
        public readonly string Formatted;
        public readonly bool RequiredEnclosingParentheses;

        private FormattedString(string formatted, bool requiredEnclosingParentheses)
        {
            this.Formatted = formatted;
            this.RequiredEnclosingParentheses = requiredEnclosingParentheses;
        }

        public static implicit operator FormattedString(string formatted) =>
            new FormattedString(formatted, false);

        public static FormattedString RequiredEnclosing(string formatted) =>
            new FormattedString(formatted, true);
    }
}
