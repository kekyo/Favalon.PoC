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
                    if (memoizedExpressions.TryGetValue(variable, out var memoized)) 
                    {
                        if (memoized.Equals(variable))
                        {
                            return memoized;
                        }
                        else if (memoized is LambdaExpression lambda)
                        {
                            var parameter = ((lambda.Parameter is VariableExpression p) ? this.Lookup(p) : null) ?? lambda.Parameter;
                            var expression = ((lambda.Expression is VariableExpression e) ? this.Lookup(e) : null) ?? lambda.Expression;
                            //var higherOrder = ((lambda.HigherOrder is VariableExpression ho) ? this.Lookup(ho) : null) ?? lambda.HigherOrder;
                            //var newLambda = new LambdaExpression(parameter, expression, higherOrder);

                            var newLambda = new LambdaExpression(parameter, expression, lambda.HigherOrder);

                            foundSymbol = variable;
                            foundExpression = newLambda;
                        }
                        else
                        {
                            foundSymbol = variable;
                            foundExpression = memoized;
                             
                            currentSymbol = memoized;
                            continue;
                        }
                    }
                }

                // Make faster when updates with short circuit.
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
