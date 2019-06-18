using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class ExpressionEnvironment
    {
        private sealed class IndexHolder
        {
            private int index;
            public int Next() =>
                index++;
        }

        private readonly ExpressionEnvironment? parent;
        private Dictionary<string, Expression>? bindExpressions;
        private IndexHolder indexHolder;

        private ExpressionEnvironment(ExpressionEnvironment parent)
        {
            this.parent = parent;
            this.indexHolder = parent.indexHolder;
        }

        public ExpressionEnvironment() =>
            this.indexHolder = new IndexHolder();

        internal ExpressionEnvironment NewScope() =>
            new ExpressionEnvironment(this);

        public bool TryGetVariable(string name, out Expression expression)
        {
            var current = (ExpressionEnvironment?)this;
            do
            {
                if (current.bindExpressions != null)
                {
                    if (current.bindExpressions.TryGetValue(name, out expression))
                    {
                        return true;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            expression = default;
            return false;
        }

        public void AddVariable(string name, Expression expression)
        {
            if (this.bindExpressions == null)
            {
                this.bindExpressions = new Dictionary<string, Expression>();
            }

            this.bindExpressions[name] = expression;
        }

        public PlaceholderExpression CreatePlaceholder() =>
            new PlaceholderExpression(indexHolder.Next());

        internal Expression Resolve(Expression expression)
        {
            // TODO:
            return expression;
        }
    }
}
