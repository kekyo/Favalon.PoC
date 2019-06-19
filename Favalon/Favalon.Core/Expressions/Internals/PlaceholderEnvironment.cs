using System.Collections.Generic;

namespace Favalon.Expressions.Internals
{
    internal sealed class PlaceholderEnvironment
    {
        private readonly Dictionary<PlaceholderExpression, Expression> placeholders =
            new Dictionary<PlaceholderExpression, Expression>();
        private int index;

        public void Reset()
        {
            placeholders.Clear();
            index = 0;
        }

        public bool TryGetExpression(PlaceholderExpression placeholder, out Expression resolved) =>
            placeholders.TryGetValue(placeholder, out resolved);

        public PlaceholderExpression Create() =>
            new PlaceholderExpression(index++);

        public void SetExpression(PlaceholderExpression placeholder, Expression expression) =>
            placeholders[placeholder] = expression;
    }
}
