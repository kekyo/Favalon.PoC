using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class InferContext
    {
        private readonly Dictionary<PlaceholderExpression, Expression> placeholders =
            new Dictionary<PlaceholderExpression, Expression>();
        private int index;

        internal InferContext() { }

        public PlaceholderExpression CreatePlaceholder() =>
            new PlaceholderExpression(index++);

        public void UnifyExpression(Expression expression1, Expression expression2)
        {
            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (expression2 is PlaceholderExpression placeholder2)
                {
                    if (!placeholder1.Equals(placeholder2))
                    {
                        placeholders[placeholder1] = expression2;
                    }
                    return;
                }
                else if (placeholders.TryGetValue(placeholder1, out var resolved))
                {
                    this.UnifyExpression(expression2, resolved);
                }
                else
                {
                    placeholders[placeholder1] = expression2;
                }
            }
            else if (expression2 is PlaceholderExpression placeholder2)
            {
                if (placeholders.TryGetValue(placeholder2, out var resolved))
                {
                    this.UnifyExpression(expression1, resolved);
                }
                else
                {
                    placeholders[placeholder2] = expression1;
                }
            }
        }

        public Expression Fixup(Expression expression)
        {
            if (expression is PlaceholderExpression placeholder)
            {
                if (placeholders.TryGetValue(placeholder, out var resolved))
                {
                    return resolved;
                }
            }

            return expression.FixupChildren(this);
        }
    }
}
