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

        public IEnumerable<PlaceholderExpression> TouchedPlaceholders =>
            touchedPlaceholders;

        internal void TouchedInResolving(PlaceholderExpression placeholder) =>
            touchedPlaceholders.Add(placeholder);

        public override string ToString() =>
            string.Join(",", touchedPlaceholders.Select(touched => $"'{touched.ReadableString}"));
    }
}
