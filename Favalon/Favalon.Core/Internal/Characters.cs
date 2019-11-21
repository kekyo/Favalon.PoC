using Favalon.Tokens;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Favalon.Internal
{
    internal static class Characters
    {
        internal static readonly Dictionary<char, ParenthesisPair> openParenthesis;
        internal static readonly Dictionary<char, ParenthesisPair> closeParenthesis;

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
                ch => new ParenthesisPair(ch, (char)(ch + 1)));
            openParenthesis.Add('[', new ParenthesisPair('[', ']'));
            openParenthesis.Add('{', new ParenthesisPair('{', '}'));

            closeParenthesis = parenthesis.ToDictionary(
                ch => (char)(ch + 1),
                ch => new ParenthesisPair(ch, (char)(ch + 1)));
            closeParenthesis.Add(']', new ParenthesisPair('[', ']'));
            closeParenthesis.Add('}', new ParenthesisPair('{', '}'));
        }

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static NumericalSignes? IsNumericSign(char ch) =>
            ch switch
            {
                '+' => (NumericalSignes?)NumericalSignes.Plus,
                '-' => NumericalSignes.Minus,
                _ => null
            };

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParenthesisPair? IsOpenParenthesis(char ch) =>
            openParenthesis.TryGetValue(ch, out var p) ? (ParenthesisPair?)p : null;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParenthesisPair? IsCloseParenthesis(char ch) =>
            closeParenthesis.TryGetValue(ch, out var p) ? (ParenthesisPair?)p : null;

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsOperator(char ch) =>
            operatorChars.Contains(ch);
    }
}
