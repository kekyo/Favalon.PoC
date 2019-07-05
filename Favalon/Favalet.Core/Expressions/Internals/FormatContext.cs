using System.Collections.Generic;

namespace Favalet.Expressions.Internals
{
    public enum FormatAnnotations
    {
        Without,
        Strict,
        SuppressPseudoes
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
            FormatAnnotations formatAnnotation, FormatNamings formatNaming,
            SortedDictionary<PlaceholderExpression, int> freeVariables)
        {
            this.FormatAnnotation = formatAnnotation;
            this.FormatNaming = formatNaming;
            this.freeVariables = freeVariables;
        }

        public readonly FormatAnnotations FormatAnnotation;
        public readonly FormatNamings FormatNaming;

        internal FormatContext(FormatAnnotations formatAnnotation, FormatNamings formatNaming) :
            this(formatAnnotation, formatNaming, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatContext NewDerived(FormatAnnotations? formatAnnotation, FormatNamings? formatNaming) =>
            new FormatContext(formatAnnotation ?? this.FormatAnnotation, formatNaming ?? this.FormatNaming, freeVariables);

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
