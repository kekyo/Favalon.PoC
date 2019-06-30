using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index;
        private readonly Dictionary<PlaceholderExpression, TermExpression> relatedExpressions =
            new Dictionary<PlaceholderExpression, TermExpression>();

        [DebuggerStepThrough]
        public PlaceholderExpression Create(TermExpression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);

        [DebuggerStepThrough]
        public void AddRelated(PlaceholderExpression placeholder, TermExpression expression) =>
            relatedExpressions.Add(placeholder, expression);

        [DebuggerStepThrough]
        public TermExpression? GetRelated(PlaceholderExpression placeholder) =>
            relatedExpressions.TryGetValue(placeholder, out var expression) ? expression : null;

        [DebuggerStepThrough]
        public void Remove(IEnumerable<PlaceholderExpression> placeholders)
        {
            foreach (var placeholder in placeholders)
            {
                relatedExpressions.Remove(placeholder);
            }
        }
    }
}
