﻿using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    public sealed class FormatContext
    {
        private readonly SortedDictionary<PlaceholderExpression, int> freeVariables;

        public readonly bool WithAnnotation;
        public readonly bool FancySymbols;
        public readonly bool StrictNaming;

        private FormatContext(
            bool withAnnotation, bool strictNaming, bool fancySymbols,
            SortedDictionary<PlaceholderExpression, int> freeVariables)
        {
            this.WithAnnotation = withAnnotation;
            this.FancySymbols = fancySymbols;
            this.StrictNaming = strictNaming;
            this.freeVariables = freeVariables;
        }

        internal FormatContext(bool withAnnotation, bool strictNaming, bool fancySymbols) :
            this(withAnnotation, strictNaming, fancySymbols, new SortedDictionary<PlaceholderExpression, int>())
        {
        }

        public FormatContext NewDerived(bool? withAnnotation, bool? fancySymbols) =>
            new FormatContext(withAnnotation ?? this.WithAnnotation, this.StrictNaming, fancySymbols ?? this.FancySymbols, freeVariables);

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
