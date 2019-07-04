using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;
        private readonly Dictionary<VariableExpression, Expression> memoizedExpressions =
            new Dictionary<VariableExpression, Expression>();

        public PlaceholderController()
        { }

        public PlaceholderExpression Create(Expression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);

        public void Memoize(VariableExpression symbol, Expression expression) =>
            memoizedExpressions.Add(symbol, expression);

        public Expression? Lookup(VariableExpression symbol)
        {
            Expression currentSymbol = symbol;
            VariableExpression? foundSymbol = null;
            Expression? foundExpression = null;
            while (true)
            {
                if (currentSymbol is VariableExpression variable)
                {
                    if (memoizedExpressions.TryGetValue(variable, out var expression)) 
                    {
                        if (expression.Equals(variable))
                        {
                            return expression;
                        }
                        else
                        {
                            foundSymbol = variable;
                            foundExpression = expression;
                             
                            currentSymbol = expression;
                            continue;
                        }
                    }
                }

                // Make short circuit.
                if (foundSymbol != null)
                {
                    Debug.Assert(foundExpression != null);
                    memoizedExpressions[foundSymbol] = foundExpression;
                }

                return foundExpression;
            }
        }
    }
}
