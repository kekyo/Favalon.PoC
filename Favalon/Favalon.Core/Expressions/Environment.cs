using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private sealed class IndexCell
        {
            private long index;
            public long Next() =>
                index++;
        }

        private readonly Environment? parent;
        private Dictionary<string, Expression>? namedExpressions;
        private IndexCell indexCell;

        private Environment(Environment parent, IndexCell indexCell)
        {
            this.parent = parent;
            this.indexCell = indexCell;
        }

        public Environment() =>
            indexCell = new IndexCell();

        public void Reset() =>
            namedExpressions = null;

        internal Environment NewScope() =>
            new Environment(this, indexCell);

        public PlaceholderExpression CreatePlaceholder() =>
            new PlaceholderExpression(indexCell.Next());

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
