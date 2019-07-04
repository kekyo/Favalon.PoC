using System.Collections.Generic;

namespace Favalet.Expressions.Internals
{
    public struct FormattedString
    {
        public readonly string Formatted;
        public readonly bool RequiredEncloseParentheses;

        private FormattedString(string formatted, bool requiredEncloseParentheses)
        {
            this.Formatted = formatted;
            this.RequiredEncloseParentheses = requiredEncloseParentheses;
        }

        public static implicit operator FormattedString(string formatted) =>
            new FormattedString(formatted, false);

        public static FormattedString RequiredEnclose(string formatted) =>
            new FormattedString(formatted, true);
    }

    public enum FormatNamings
    {
        Standard,
        Strict,
        Fancy
    }

    public sealed class FormatContext
    {
        private readonly SortedDictionary<PlaceholderExpression, int> freeVariables;

        private FormatContext(
            bool withAnnotation, FormatNamings formatNaming,
            SortedDictionary<PlaceholderExpression, int> freeVariables)
        {
            this.WithAnnotation = withAnnotation;
            this.FormatNaming = formatNaming;
            this.freeVariables = freeVariables;
        }

        public readonly bool WithAnnotation;
        public readonly FormatNamings FormatNaming;

        internal FormatContext(bool withAnnotation, FormatNamings formatNaming) :
            this(withAnnotation, formatNaming, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatContext NewDerived(bool? withAnnotation, FormatNamings? formatNaming) =>
            new FormatContext(withAnnotation ?? this.WithAnnotation,formatNaming ?? this.FormatNaming, freeVariables);

        internal int GetAdjustedIndex(PlaceholderExpression freeVariable)
        {
            if (!freeVariables.TryGetValue(freeVariable, out var index))
            {
                index = freeVariables.Count;
                freeVariables.Add(freeVariable, index);
            }
            return index;
        }
    }
}
