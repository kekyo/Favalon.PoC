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

using Favalet.Internals;
using Favalet.Terms;
using Favalet.Terms.Basis;
using Favalet.Terms.Internals;
using Favalet.Terms.Specialized;
using System.Collections.Generic;

namespace Favalet
{
    public sealed partial class Terrain :
        Term.IInferringContext, Term.IResolvingContext
    {
        private readonly PlaceholderController placeholderController = new PlaceholderController();
        private readonly List<InferErrorInformation> errorInformations = new List<InferErrorInformation>();

        private Terrain()
        { }

#line hidden
        private PlaceholderTerm CreatePlaceholder(Term higherOrder, TextRange textRange) =>
            placeholderController.Create(higherOrder, textRange);
        PlaceholderTerm Term.IInferringContext.CreatePlaceholder(Term higherOrder, TextRange textRange) =>
            placeholderController.Create(higherOrder, textRange);

        public void Bind(BoundVariableTerm bound, Term term) =>
            placeholderController.Memoize(bound, term);
        void Term.IInferringContext.Memoize(SymbolicVariableTerm symbol, Term term) =>
            placeholderController.Memoize(symbol, term);

        Term? Term.IInferringContext.Lookup(VariableTerm symbol) =>
            placeholderController.Lookup(this, symbol);
        Term? Term.IResolvingContext.Lookup(VariableTerm symbol) =>
            placeholderController.Lookup(this, symbol);

        private TTerm Visit<TTerm>(TTerm term, Term higherOrderHint)
            where TTerm : Term =>
            (TTerm)term.InternalVisitInferring(this, higherOrderHint);
        TTerm Term.IInferringContext.Visit<TTerm>(TTerm term, Term higherOrderHint) =>
            (TTerm)term.InternalVisitInferring(this, higherOrderHint);
        TTerm Term.IResolvingContext.Visit<TTerm>(TTerm term) =>
            (TTerm)term.InternalVisitResolving(this);

        internal Term RecordError(string details, Term primaryTerm, params Term[] terms)
        {
            errorInformations.Add(InferErrorInformation.Create(details, primaryTerm, terms));
            return primaryTerm;
        }

        Term Term.IInferringContext.RecordError(string details, Term primaryTerm, Term[] terms) =>
            this.RecordError(details, primaryTerm, terms);

        public InferResult<Term> Infer(Term term, Term higherOrderHint) =>
            this.Infer<Term>(term, higherOrderHint);
#line default

        public InferResult<TTerm> Infer<TTerm>(TTerm term, Term higherOrderHint)
            where TTerm : Term
        {
            var partial = term.InternalVisitInferring(this, higherOrderHint);
            var inferred = partial.InternalVisitResolving(this);
            var result = InferResult<TTerm>.Create((TTerm)inferred, errorInformations.ToArray());
            errorInformations.Clear();
            return result;
        }

        public static Terrain Create() =>
            new Terrain();
    }
}
