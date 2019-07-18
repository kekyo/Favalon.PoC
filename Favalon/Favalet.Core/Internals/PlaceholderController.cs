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

using Favalet.Expressions;
using Favalet.Expressions.Basis;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;
        private readonly Dictionary<VariableExpression, Expression> memoizedExpressions =
            new Dictionary<VariableExpression, Expression>();

        public PlaceholderController()
        { }

        public PlaceholderExpression Create(Expression higherOrder, TextRange textRange) =>
            new PlaceholderExpression(index++, higherOrder, textRange);

        public void Memoize(VariableExpression symbol, Expression expression) =>
            memoizedExpressions.Add(symbol, expression);

        private Expression? Lookup(
            Terrain terrain, VariableExpression symbol0, VariableExpression symbol, HashSet<VariableExpression> collected)
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
                            return terrain.RecordError(
                                $"Recursive unification problem: {symbol0.StrictReadableString} ... {memoized.StrictReadableString}",
                                symbol0,
                                memoized);
                        }

                        if (memoized is LambdaExpression lambda)
                        {
                            var parameter = ((lambda.Parameter is VariableExpression p) ?
                                this.Lookup(terrain, symbol0, p, new HashSet<VariableExpression>(collected)) :
                                null) ??
                                lambda.Parameter;
                            var expression = ((lambda.Expression is VariableExpression e) ?
                                this.Lookup(terrain, symbol0, e, new HashSet<VariableExpression>(collected)) :
                                null) ??
                                lambda.Expression;

                            var newLambda = LambdaExpression.Create(parameter, expression, true, lambda.TextRange);

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

        public Expression? Lookup(Terrain terrain, VariableExpression symbol) =>
            this.Lookup(terrain, symbol, symbol, new HashSet<VariableExpression>());
    }
}
