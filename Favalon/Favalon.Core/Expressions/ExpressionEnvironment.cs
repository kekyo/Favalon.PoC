using Favalon.Expressions.Internals;
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
        private Dictionary<string, Expression>? boundExpressions;
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

        public PlaceholderExpression CreatePlaceholder(TextRange textRange) =>
            new PlaceholderExpression(indexCell.Next(), textRange);

        internal bool TryGetBoundExpression(string boundName, out Expression expression)
        {
            ExpressionEnvironment? current = this;
            do
            {
                if (current.boundExpressions != null)
                {
                    if (current.boundExpressions.TryGetValue(boundName, out expression))
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
            if ((expression is VariableExpression variable) && (variable.Name == boundName))
            {
                throw new System.InvalidOperationException();
            }

            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<string, Expression>();
            }
            boundExpressions[boundName] = expression;
        }

        public void Bind(FreeVariableExpression bound, Expression expression)
        {
            var context = new InferContext();
            var (b, e) = BindExpression.InternalVisit(this, context, bound, expression);
            b = context.FixupHigherOrders(b, 0);
            e = context.FixupHigherOrders(e, 0);

            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<string, Expression>();
            }
            boundExpressions[b.Name] = e;
        }

        public override string ToString() =>
            string.Format("Scope [{0}]: {1}",
                this.parent!.Traverse(environemnt => environemnt.parent!).Count(),
                string.Join(", ", this.
                    Traverse(environemnt => environemnt.parent!).
                    Where(environment => environment.boundExpressions != null).
                    SelectMany(environment => environment.boundExpressions).
                    GroupBy(entry => entry.Key, entry => entry.Value).
                    Select(g => $"{g.Key}={g.First().ReadableString}")));

        public static ExpressionEnvironment Create() =>
            new ExpressionEnvironment();
    }
}
