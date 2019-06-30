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
            relatedExpressions[placeholder] = expression;

        public TermExpression? GetRelated(PlaceholderExpression placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (relatedExpressions.TryGetValue(current, out var expression))
                {
                    if (expression is PlaceholderExpression p)
                    {
                        current = p;
                    }
                    else
                    {
                        return expression;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public void Remove(IEnumerable<PlaceholderExpression> placeholders)
        {
            // TODO: We have to except implicitly depending between the placeholder and another placeholder.

            // A --> B   (X)
            // B --> C   (X)
            // D --> B   * missed B
            // Remove list: A, B

            foreach (var placeholder in placeholders)
            {
                relatedExpressions.Remove(placeholder);
            }
        }
    }
}
