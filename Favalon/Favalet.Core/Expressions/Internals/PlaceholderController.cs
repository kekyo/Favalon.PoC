// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private Expression? Lookup(VariableExpression symbol0, VariableExpression symbol, HashSet<VariableExpression> collected)
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

                        if (!collected.Add(variable))
                        {
                            throw new ArgumentException(
                                $"Recursive unification problem: {symbol0.StrictReadableString} ... {memoized.StrictReadableString}");
                        }

                        if (memoized is LambdaExpression lambda)
                        {
                            var parameter = ((lambda.Parameter is VariableExpression p) ?
                                this.Lookup(symbol0, p, new HashSet<VariableExpression>(collected)) :
                                null) ??
                                lambda.Parameter;
                            var expression = ((lambda.Expression is VariableExpression e) ?
                                this.Lookup(symbol0, e, new HashSet<VariableExpression>(collected)) :
                                null) ??
                                lambda.Expression;

                            var newLambda = LambdaExpression.Create(parameter, expression, true);

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

        public Expression? Lookup(VariableExpression symbol) =>
            this.Lookup(symbol, symbol, new HashSet<VariableExpression>());
    }
}
