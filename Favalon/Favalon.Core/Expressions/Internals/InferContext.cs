using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class InferContext
    {
        private readonly HashSet<PlaceholderExpression> touchedPlaceholders =
            new HashSet<PlaceholderExpression>();

        [DebuggerStepThrough]
        internal InferContext()
        { }

        public int Rank { get; private set; }

        public IEnumerable<PlaceholderExpression> TouchedPlaceholders =>
            touchedPlaceholders;

        [DebuggerStepThrough]
        internal void RaiseRank() =>
            this.Rank++;
        [DebuggerStepThrough]
        internal void DropRank() =>
            this.Rank--;

        internal void TouchedInResolving(PlaceholderExpression placeholder) =>
            touchedPlaceholders.Add(placeholder);

        public override string ToString()
        {
            var touched = string.Join(",", touchedPlaceholders.Select(touched => $"'{touched.ReadableString}"));
            return $"Rank={this.Rank}, Touched=[{touched}]";
        }
    }
}
