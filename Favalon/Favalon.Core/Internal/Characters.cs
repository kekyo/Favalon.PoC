using Favalon.Tokens;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Favalon.Internal
{
    internal struct ParenthesisInformation
    {
        public readonly char Open;
        public readonly char Close;

        public ParenthesisInformation(char open, char close)
        {
            this.Open = open;
            this.Close = close;
        }
    }

    internal static class Characters
    {
        internal static readonly Dictionary<char, ParenthesisInformation> openParenthesis;
        internal static readonly Dictionary<char, ParenthesisInformation> closeParenthesis;

        internal static readonly HashSet<char> operatorChars = new HashSet<char>
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''' */, /* '(', ')', */
            '*', '+', ',', '-'/* , '.'*/, '/'/*, ':' */, ';', '<', '=', '>', '?',
            '@', /* '[', */ '\\', /* ']', */ '^', '_', '`', /* '{', */ '|', /* '}', */ '~'
        };

        static Characters()
        {
            // TODO: generate statically
            var parenthesis =
                Enumerable.Range(0x20, ushort.MaxValue - 1).
#if NET40 || NET45
                AsParallel().
#endif
                Where(value =>
                    (CharUnicodeInfo.GetUnicodeCategory((char)value) == UnicodeCategory.OpenPunctuation) &&
                    (CharUnicodeInfo.GetUnicodeCategory((char)(value + 1)) == UnicodeCategory.ClosePunctuation)).
                Select(value => (char)value).
                ToArray();

            openParenthesis = parenthesis.ToDictionary(
                ch => ch,
                ch => new ParenthesisInformation(ch, (char)(ch + 1)));
            openParenthesis.Add('[', new ParenthesisInformation('[', ']'));
            openParenthesis.Add('{', new ParenthesisInformation('{', '}'));

            closeParenthesis = parenthesis.ToDictionary(
                ch => (char)(ch + 1),
                ch => new ParenthesisInformation(ch, (char)(ch + 1)));
            closeParenthesis.Add(']', new ParenthesisInformation('[', ']'));
            closeParenthesis.Add('}', new ParenthesisInformation('{', '}'));
        }

        public static Signs? IsNumericSign(char ch) =>
            ch switch
            {
                '+' => (Signs?)Signs.Plus,
                '-' => Signs.Minus,
                _ => null
            };

        public static ParenthesisInformation? IsOpenParenthesis(char ch) =>
            openParenthesis.TryGetValue(ch, out var p) ? (ParenthesisInformation?)p : null;

        public static ParenthesisInformation? IsCloseParenthesis(char ch) =>
            closeParenthesis.TryGetValue(ch, out var p) ? (ParenthesisInformation?)p : null;

        public static bool IsOperator(char ch) =>
            operatorChars.Contains(ch);
    }
}
