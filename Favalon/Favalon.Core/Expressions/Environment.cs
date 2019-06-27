using Favalon.Expressions.Internals;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private sealed class IndexCell
        {
            private long index;
            public long Next() =>
                index++;
            public override string ToString() =>
                $"Index={index}";
        }

        private readonly Environment? parent;
        private Dictionary<string, Expression>? boundExpressions;
        private IndexCell indexCell;

        private Environment(Environment parent, IndexCell indexCell)
        {
            this.parent = parent;
            this.indexCell = indexCell;
        }

        private Environment() =>
            indexCell = new IndexCell();

        public void Reset() =>
            boundExpressions = null;

        internal Environment NewScope() =>
            new Environment(this, indexCell);

        public PlaceholderExpression CreatePlaceholder(TextRange textRange) =>
            new PlaceholderExpression(indexCell.Next(), textRange);

        internal bool TryGetBoundExpression(string boundName, out Expression expression)
        {
            Environment? current = this;
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

        public void Bind(FreeVariableExpression bound, Expression expression)
        {
            var context = new InferContext();
            var (b, e) = BindExpression.InternalVisit(this, context, bound, expression);
            var fb = context.FixupHigherOrders(b, 0);
            var fe = context.FixupHigherOrders(e, 0);

            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<string, Expression>();
            }
            boundExpressions[fb.Name] = fe;
        }

        internal void RegisterVariable(FreeVariableExpression freeVariable) =>
            this.Bind(freeVariable, new NamedPlaceholderExpression(freeVariable.Name, freeVariable.HigherOrder, freeVariable.TextRange));

        public override string ToString() =>
            string.Format("Scope {0}: [{1}]",
                this.parent!.Traverse(environemnt => environemnt.parent!).Count(),
                string.Join(", ", this.
                    Traverse(environemnt => environemnt.parent!).
                    Where(environment => environment.boundExpressions != null).
                    SelectMany(environment => environment.boundExpressions).
                    GroupBy(entry => entry.Key, entry => entry.Value).
                    Select(g => $"{g.Key}=>{g.First().ReadableString}")));

        public static Environment Create() =>
            new Environment();
    }
}
