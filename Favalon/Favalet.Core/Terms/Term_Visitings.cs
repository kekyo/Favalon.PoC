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

using Favalet.Terms.Specialized;

namespace Favalet.Terms
{
    partial class Term
    {
        protected internal interface IInferringContext
        {
            PlaceholderTerm CreatePlaceholder(Term higherOrder, TextRange textRange);

            void Memoize(SymbolicVariableTerm symbol, Term term);

            Term? Lookup(VariableTerm symbol);

            Term Unify(Term term1, Term term2);

            Term RecordError(string details, Term primaryTerm, params Term[] terms);

            TTerm Visit<TTerm>(TTerm term, Term higherOrderHint)
                where TTerm : Term;
        }

        protected internal interface IResolvingContext
        {
            Term? Lookup(VariableTerm symbol);

            TTerm Visit<TTerm>(TTerm term)
                where TTerm : Term;
        }

#line hidden
        internal Term InternalVisitInferring(IInferringContext context, Term higherOrderHint) =>
            this.VisitInferring(context, higherOrderHint);
        internal Term InternalVisitResolving(IResolvingContext context) =>
            this.VisitResolving(context);
#line default
    }
}
