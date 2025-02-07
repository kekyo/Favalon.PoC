﻿using Favalon.Tokens;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Favalon.Internal
{
    internal static class StringUtilities
    {
        internal static readonly Dictionary<char, ParenthesisPair> openParenthesis;
        internal static readonly Dictionary<char, ParenthesisPair> closeParenthesis;

        internal static readonly HashSet<char> operatorChars = new HashSet<char>
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''' */, /* '(', ')', */
            '*', '+', ',', '-'/* , '.'*/, '/'/*, ':' */, ';', '<', '=', '>', '?',
            '@', /* '[', */ '\\', /* ']', */ '^', '_', '`', /* '{', */ '|', /* '}', */ '~'
        };

        static StringUtilities()
        {
            // TODO: generate statically
            var parenthesis =
                Enumerable.Range(0x20, ushort.MaxValue - 1).
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

#if NET35
        public static string Join(string separator, IEnumerable<string> values) =>
            string.Join(separator, values.ToArray());

        public static void Clear(this StringBuilder sb) =>
            sb.Length = 0;
#else
#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Join(string separator, IEnumerable<string> values) =>
            string.Join(separator, values);
#endif

#if NET45 || NETSTANDARD1_0
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Append(this string lhs, string rhs) =>
            lhs + rhs;
    }
}
