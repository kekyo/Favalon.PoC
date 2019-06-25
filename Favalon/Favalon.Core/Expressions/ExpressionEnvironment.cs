using System.Collections.Generic;

namespace Favalon.Expressions
{
    public sealed class ExpressionEnvironment
    {
        private sealed class IndexCell
        {
            private long index;
            public long Next() =>
                index++;
            public override string ToString() =>
                $"Index={index}";
        }

        private readonly ExpressionEnvironment? parent;
        private Dictionary<string, Expression>? bindExpressions;
        private IndexCell indexCell;

        private ExpressionEnvironment(ExpressionEnvironment parent, IndexCell indexCell)
        {
            this.parent = parent;
            this.indexCell = indexCell;
        }

        private ExpressionEnvironment() =>
            indexCell = new IndexCell();

        public void Reset() =>
            bindExpressions = null;

        internal ExpressionEnvironment NewScope() =>
            new ExpressionEnvironment(this, indexCell);

        public FreeVariableExpression CreateFreeVariable(TextRange textRange) =>
            new FreeVariableExpression(indexCell.Next(), textRange);

        internal bool TryGetBoundExpression(string boundName, out Expression expression)
        {
            ExpressionEnvironment? current = this;
            do
            {
                if (current.bindExpressions != null)
                {
                    if (current.bindExpressions.TryGetValue(boundName, out expression))
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

        public void Bind(string boundName, Expression expression)
        {
            if (bindExpressions == null)
            {
                bindExpressions = new Dictionary<string, Expression>();
            }
            bindExpressions[boundName] = expression;
        }

        public static ExpressionEnvironment Create() =>
            new ExpressionEnvironment();
    }
}
