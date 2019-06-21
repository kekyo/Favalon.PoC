using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private readonly Environment? parent;
        private Dictionary<string, Expression>? namedExpressions;

        private Environment(Environment parent)
        {
            this.parent = parent;
        }

        public Environment() { }

        public void Reset() =>
            namedExpressions = null;

        internal Environment NewScope() =>
            new Environment(this);

        internal bool TryGetNamedExpression(string name, out Expression expression)
        {
            Environment? current = this;
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
