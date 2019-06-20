using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class ExpressionEnvironment
    {
        private readonly ExpressionEnvironment? parent;
        private Dictionary<string, Expression>? namedExpressions;

        private ExpressionEnvironment(ExpressionEnvironment parent)
        {
            this.parent = parent;
        }

        public ExpressionEnvironment() { }

        public void Reset() =>
            namedExpressions = null;

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
    }
}
