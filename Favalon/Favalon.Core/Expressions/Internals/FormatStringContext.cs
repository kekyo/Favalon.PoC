using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    public sealed class FormatStringContext
    {
        private SortedDictionary<PlaceholderExpression, int> placeholders;

        public readonly bool WithAnnotation;
        public readonly bool FancySymbols;
        public readonly bool StrictNaming;

        private FormatStringContext(bool withAnnotation, bool strictNaming, bool fancySymbols, SortedDictionary<PlaceholderExpression, int> placeholders)
        {
            this.WithAnnotation = withAnnotation;
            this.FancySymbols = fancySymbols;
            this.StrictNaming = strictNaming;
            this.placeholders = placeholders;
        }

        internal FormatStringContext(bool withAnnotation, bool strictNaming, bool fancySymbols) :
            this(withAnnotation, strictNaming, fancySymbols, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatStringContext NewDerived(bool? withAnnotation, bool? fancySymbols) =>
            new FormatStringContext(withAnnotation ?? this.WithAnnotation, this.StrictNaming, fancySymbols ?? this.FancySymbols, placeholders);

        internal int GetAdjustedIndex(PlaceholderExpression placeholder)
        {
            if (!placeholders.TryGetValue(placeholder, out var index))
            {
                index = placeholders.Count;
                placeholders.Add(placeholder, index);
            }
            return index;
        }
    }
}
