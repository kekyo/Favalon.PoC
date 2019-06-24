using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    public sealed class FormatStringContext
    {
        private SortedDictionary<PlaceholderExpression, int> placeholders;

        public readonly bool WithAnnotation;
        public readonly bool Fancy;
        public readonly bool StrictPlaceholderNaming;

        private FormatStringContext(bool withAnnotation, bool strictPlaceholderNaming, bool fancy, SortedDictionary<PlaceholderExpression, int> placeholders)
        {
            this.WithAnnotation = withAnnotation;
            this.Fancy = fancy;
            this.StrictPlaceholderNaming = strictPlaceholderNaming;
            this.placeholders = placeholders;
        }

        internal FormatStringContext(bool withAnnotation, bool strictPlaceholderNaming, bool fancy) :
            this(withAnnotation, strictPlaceholderNaming, fancy, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatStringContext NewDerived(bool? withAnnotation, bool? fancy) =>
            new FormatStringContext(withAnnotation ?? this.WithAnnotation, this.StrictPlaceholderNaming, fancy ?? this.Fancy, placeholders);

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
