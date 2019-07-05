using System.Collections.Generic;

namespace Favalet.Expressions.Internals
{
    public sealed class FormatContext
    {
        private readonly SortedDictionary<PlaceholderExpression, int> freeVariables;

        private FormatContext(
            FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
            SortedDictionary<PlaceholderExpression, int> freeVariables)
        {
            this.FormatAnnotation = formatAnnotation;
            this.FormatNaming = formatNaming;
            this.FormatOperator = formatOperator;
            this.freeVariables = freeVariables;
        }

        public readonly FormatAnnotations FormatAnnotation;
        public readonly FormatNamings FormatNaming;
        public readonly FormatOperators FormatOperator;

        internal FormatContext(FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator) :
            this(formatAnnotation, formatNaming, formatOperator, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatContext NewDerived(
            FormatAnnotations? formatAnnotation, FormatNamings? formatNaming, FormatOperators? formatOperator) =>
            new FormatContext(
                formatAnnotation ?? this.FormatAnnotation,
                formatNaming ?? this.FormatNaming,
                formatOperator ?? this.FormatOperator,
                freeVariables);

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
