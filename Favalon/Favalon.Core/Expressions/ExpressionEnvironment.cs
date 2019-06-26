using System.Collections.Generic;
using System.Linq;

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
        private Dictionary<string, (VariableExpression bound, Expression expression)>? boundExpressions;
        private IndexCell indexCell;

        private ExpressionEnvironment(ExpressionEnvironment parent, IndexCell indexCell)
        {
            this.parent = parent;
            this.indexCell = indexCell;
        }

        private ExpressionEnvironment() =>
            indexCell = new IndexCell();

        public void Reset() =>
            boundExpressions = null;

        internal ExpressionEnvironment NewScope() =>
            new ExpressionEnvironment(this, indexCell);

        public FreeVariableExpression CreateFreeVariable(TextRange textRange) =>
            new FreeVariableExpression(indexCell.Next(), textRange);

        internal (VariableExpression bound, Expression expression)? GetBoundExpression(string boundName)
        {
            ExpressionEnvironment? current = this;
            do
            {
                if (current.boundExpressions != null)
                {
                    if (current.boundExpressions.TryGetValue(boundName, out var resolved))
                    {
                        return resolved;
                    }
                }
                current = current.parent;
            }
            while (current != null);

            return default;
        }

        public void Bind(VariableExpression bound, Expression expression)
        {
            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<string, (VariableExpression bound, Expression expression)>();
            }
            boundExpressions[bound.Name] = (bound, expression);
        }

        public override string ToString() =>
            string.Format("Scope [{0}]: {1}",
                this.parent!.Traverse(environemnt => environemnt.parent!).Count(),
                string.Join(", ", this.
                    Traverse(environemnt => environemnt.parent!).
                    Where(environment => environment.boundExpressions != null).
                    SelectMany(environment => environment.boundExpressions).
                    GroupBy(entry => entry.Key, entry => entry.Value).
                    Select(g => $"{g.Key}={g.First().bound.ReadableString}[{g.First().expression.ReadableString}]")));

        public static ExpressionEnvironment Create() =>
            new ExpressionEnvironment();
    }
}
