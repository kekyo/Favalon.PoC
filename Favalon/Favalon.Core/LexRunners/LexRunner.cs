using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Favalon.LexRunners
{
    internal abstract class LexRunner
    {
        internal struct Parenthesis
        {
            public readonly char Left;
            public readonly char Right;

            public Parenthesis(char left, char right)
            {
                this.Left = left;
                this.Right = right;
            }
        }

        internal static readonly Dictionary<char, Parenthesis> openParenthesis;
        internal static readonly Dictionary<char, Parenthesis> closeParenthesis;

        internal static readonly HashSet<char> operatorChars = new HashSet<char>
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''' */, /* '(', ')', */
            '*', '+', ',', '-'/* , '.'*/, '/'/*, ':' */, ';', '<', '=', '>', '?',
            '@', /* '[', */ '\\', /* ']', */ '^', '_', '`', /* '{', */ '|', /* '}', */ '~'
        };

        static LexRunner()
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
                ch => new Parenthesis(ch, (char)(ch + 1)));
            openParenthesis.Add('[', new Parenthesis('[', ']'));
            openParenthesis.Add('{', new Parenthesis('{', '}'));

            closeParenthesis = parenthesis.ToDictionary(
                ch => (char)(ch + 1),
                ch => new Parenthesis(ch, (char)(ch + 1)));
            closeParenthesis.Add(']', new Parenthesis('[', ']'));
            closeParenthesis.Add('}', new Parenthesis('{', '}'));
        }

        public static bool IsNumericSign(char ch) =>
            (ch == '-') || (ch == '+');

        public static Parenthesis? IsOpenParenthesis(char ch) =>
            openParenthesis.TryGetValue(ch, out var p) ? (Parenthesis?)p : null;

        public static Parenthesis? IsCloseParenthesis(char ch) =>
            closeParenthesis.TryGetValue(ch, out var p) ? (Parenthesis?)p : null;

        public static bool IsOperator(char ch) =>
            operatorChars.Contains(ch);

        protected LexRunner()
        { }

        public abstract RunResult Run(RunContext context, char ch);

        public virtual RunResult Finish(RunContext context) =>
            RunResult.Empty(this);
    }
}
