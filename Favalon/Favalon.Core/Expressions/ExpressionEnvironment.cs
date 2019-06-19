using System.Collections.Generic;

using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class ExpressionEnvironment
    {
        private readonly PlaceholderEnvironment placeholderEnvironment;
        private readonly ExpressionEnvironment? parent;
        private Dictionary<string, Expression>? namedExpressions;

        private ExpressionEnvironment(ExpressionEnvironment parent)
        {
            this.parent = parent;
            this.placeholderEnvironment = parent.placeholderEnvironment;
        }

        public ExpressionEnvironment() =>
            this.placeholderEnvironment = new PlaceholderEnvironment();

        public void Reset()
        {
            placeholderEnvironment.Reset();
            namedExpressions = null;
        }

        internal ExpressionEnvironment NewScope() =>
            new ExpressionEnvironment(this);

        internal bool TryGetNamedExpression(string name, out Expression expression)
        {
            ExpressionEnvironment? current = this;
            do
            {
                if (current.namedExpressions != null)
                {
                    if (current.namedExpressions.TryGetValue(name, out expression))
                    {
                        return true;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            expression = default!;
            return false;
        }

        public void SetNamedExpression(string name, Expression expression)
        {
            if (namedExpressions == null)
            {
                namedExpressions = new Dictionary<string, Expression>();
            }

            namedExpressions[name] = expression;
        }

        internal PlaceholderExpression CreatePlaceholder() =>
            placeholderEnvironment.Create();

        internal void UnifyExpression(Expression expression1, Expression expression2)
        {
            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (expression2 is PlaceholderExpression placeholder2)
                {
                    if (!placeholder1.Equals(placeholder2))
                    {
                        placeholderEnvironment.SetExpression(placeholder1, expression2);
                    }
                    return;
                }
                else if (placeholderEnvironment.TryGetExpression(placeholder1, out var resolved))
                {
                    this.UnifyExpression(expression2, resolved);
                }
                else
                {
                    placeholderEnvironment.SetExpression(placeholder1, expression2);
                }
            }
            else if (expression2 is PlaceholderExpression placeholder2)
            {
                if (placeholderEnvironment.TryGetExpression(placeholder2, out var resolved))
                {
                    this.UnifyExpression(expression1, resolved);
                }
                else
                {
                    placeholderEnvironment.SetExpression(placeholder2, expression1);
                }
            }
        }

        internal Expression Resolve(Expression expression)
        {
            if (expression is PlaceholderExpression placeholder)
            {
                if (placeholderEnvironment.TryGetExpression(placeholder, out var resolved))
                {
                    return resolved;
                }
            }
            return expression;
        }
    }
}
