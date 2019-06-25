using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    public sealed class FormatStringContext
    {
        private SortedDictionary<FreeVariableExpression, int> freeVariables;

        public readonly bool WithAnnotation;
        public readonly bool FancySymbols;
        public readonly bool StrictNaming;

        private FormatStringContext(
            bool withAnnotation, bool strictNaming, bool fancySymbols,
            SortedDictionary<FreeVariableExpression, int> freeVariables)
        {
            this.WithAnnotation = withAnnotation;
            this.FancySymbols = fancySymbols;
            this.StrictNaming = strictNaming;
            this.freeVariables = freeVariables;
        }

        internal FormatStringContext(bool withAnnotation, bool strictNaming, bool fancySymbols) :
            this(withAnnotation, strictNaming, fancySymbols, new SortedDictionary<FreeVariableExpression, int>())
        {
        }

        public FormatStringContext NewDerived(bool? withAnnotation, bool? fancySymbols) =>
            new FormatStringContext(withAnnotation ?? this.WithAnnotation, this.StrictNaming, fancySymbols ?? this.FancySymbols, freeVariables);

        internal int GetAdjustedIndex(FreeVariableExpression freeVariable)
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
