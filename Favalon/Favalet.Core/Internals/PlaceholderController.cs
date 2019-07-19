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

using Favalet.Terms;
using Favalet.Terms.Basis;
using Favalet.Terms.Specialized;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;
        private readonly Dictionary<VariableTerm, Term> memoizedTerms =
            new Dictionary<VariableTerm, Term>();

        public PlaceholderController()
        { }

        public PlaceholderTerm Create(Term higherOrder, TextRange textRange) =>
            new PlaceholderTerm(index++, higherOrder, textRange);

        public void Memoize(VariableTerm symbol, Term term) =>
            memoizedTerms.Add(symbol, term);

        private Term? Lookup(
            Terrain terrain, VariableTerm symbol0, VariableTerm symbol, HashSet<VariableTerm> collected)
        {
            Term currentSymbol = symbol;
            VariableTerm? foundSymbol = null;
            Term? foundTerm = null;
            while (true)
            {
                if (currentSymbol is VariableTerm variable)
                {
                    if (memoizedTerms.TryGetValue(variable, out var memoized)) 
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

                        if (memoized is LambdaTerm lambda)
                        {
                            var parameter = ((lambda.Parameter is VariableTerm p) ?
                                this.Lookup(terrain, symbol0, p, new HashSet<VariableTerm>(collected)) :
                                null) ??
                                lambda.Parameter;
                            var term = ((lambda.Term is VariableTerm e) ?
                                this.Lookup(terrain, symbol0, e, new HashSet<VariableTerm>(collected)) :
                                null) ??
                                lambda.Term;

                            var newLambda = LambdaTerm.Create(parameter, term, true, lambda.TextRange);

                            foundSymbol = variable;
                            foundTerm = newLambda;
                        }
                        else
                        {
                            foundSymbol = variable;
                            foundTerm = memoized;
                             
                            currentSymbol = memoized;
                            continue;
                        }
                    }
                }

                // Make faster when updates with short circuit.
                if (foundSymbol != null)
                {
                    Debug.Assert(foundTerm != null);
                    memoizedTerms[foundSymbol] = foundTerm;
                }

                return foundTerm;
            }
        }

        public Term? Lookup(Terrain terrain, VariableTerm symbol) =>
            this.Lookup(terrain, symbol, symbol, new HashSet<VariableTerm>());
    }
}
