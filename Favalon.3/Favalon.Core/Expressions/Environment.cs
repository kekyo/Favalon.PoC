﻿using Favalon.Expressions.Internals;
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
        private readonly IndexCell indexCell;
        private Dictionary<FreeVariableExpression, Expression>? boundExpressions;

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
            this.CreatePlaceholder(UndefinedExpression.Instance, textRange);
        public PlaceholderExpression CreatePlaceholder(Expression higherOrder, TextRange textRange) =>
            new PlaceholderExpression(indexCell.Next(), higherOrder, textRange);

        internal bool TryGetBoundExpression(FreeVariableExpression bound, out Expression expression)
        {
            Environment? current = this;
            do
            {
                if (current.boundExpressions != null)
                {
                    if (current.boundExpressions.TryGetValue(bound, out expression))
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

        public FreeVariableExpression Bind(FreeVariableExpression bound, Expression expression)
        {
            var context = new InferContext();
            var (b, e) = BindExpression.InternalVisit(this, context, bound, expression);
            var fb = context.FixupHigherOrders(b, 0);
            var fe = context.FixupHigherOrders(e, 0);

            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<FreeVariableExpression, Expression>();
            }
            boundExpressions[fb] = fe;

            return fb;
        }

        internal FreeVariableExpression Register(FreeVariableExpression freeVariable)
        {
            var cloned = freeVariable.CloneWithPlaceholderIfUndefined(this);
            this.Bind(freeVariable, new NamedPlaceholderExpression(cloned.Name, cloned.HigherOrder, cloned.TextRange));
            return cloned;
        }

        public override string ToString() =>
            string.Format("Scope {0}: [{1}]",
                this.parent!.Traverse(environemnt => environemnt.parent!).Count(),
                string.Join(", ", this.
                    Traverse(environemnt => environemnt.parent!).
                    Where(environment => environment.boundExpressions != null).
                    SelectMany(environment => environment.boundExpressions).
                    GroupBy(entry => entry.Key, entry => entry.Value).
                    Select(g =>
                    {
                        var expression = g.First();
                        return (expression is NamedPlaceholderExpression) ?
                            expression.StrictReadableString :
                            $"{g.Key.StrictReadableString}=>{expression.StrictReadableString}";
                    })));

        public static Environment Create() =>
            new Environment();
    }
}
