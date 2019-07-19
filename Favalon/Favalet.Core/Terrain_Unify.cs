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
using System;
using System.Diagnostics;

namespace Favalet
{
    partial class Terrain
    {
        private Term UnifyLambda(LambdaTerm lambda1, LambdaTerm lambda2)
        {
            Debug.Assert((lambda1 is Term) && (lambda2 is Term));

            this.Unify(lambda1.Parameter, lambda2.Parameter);
            this.Unify(lambda1.Term, lambda2.Term);

            return lambda2;
        }

        private Term UnifyLambda(LambdaTerm lambda, Term term)
        {
            Debug.Assert((lambda is Term) && (term is Term));

            var newLambda = LambdaTerm.CreateWithPlaceholder(
                this,
                this.Unify(lambda.Parameter, UnspecifiedTerm.Instance),
                this.Unify(lambda.Term, UnspecifiedTerm.Instance),
                true,
                lambda.TextRange);

            if (term is PlaceholderTerm placeholder)
            {
                placeholderController.Memoize(placeholder, newLambda);
            }

            return lambda;
        }

        private Term UnifyPlaceholder(
            PlaceholderTerm placeholder, Term term)
        {
            Debug.Assert((placeholder is Term) && (term is Term));

            if (placeholderController.Lookup(this, placeholder) is Term lookup)
            {
                return this.Unify(lookup, term);
            }

            if (!(term is UnspecifiedTerm))
            {
                placeholderController.Memoize(placeholder, term);
            }

            return placeholder;
        }

        private Term Unify(Term term1, Term term2)
        {
            Debug.Assert((term1 is Term) && (term2 is Term));

            var result = (term1, term2) switch
            {
                (UnspecifiedTerm _, UnspecifiedTerm _) =>
                    this.CreatePlaceholder(UnspecifiedTerm.Instance, term1.TextRange),

                (Term _, Term _) when term1.Equals(term2) =>
                    term1,

                (UnspecifiedTerm _, PlaceholderTerm placeholder) =>
                    this.UnifyPlaceholder(placeholder, term1),
                (PlaceholderTerm placeholder, UnspecifiedTerm _) =>
                    this.UnifyPlaceholder(placeholder, term2),

                (Term _, PlaceholderTerm placeholder) =>
                    this.UnifyPlaceholder(placeholder, term1),
                (PlaceholderTerm placeholder, Term _) =>
                    this.UnifyPlaceholder(placeholder, term2),

                (Term _, UnspecifiedTerm _) =>
                    term1,
                (UnspecifiedTerm _, Term _) =>
                    term2,

                (LambdaTerm lambda1, LambdaTerm lambda2) =>
                    this.UnifyLambda(lambda1, lambda2),
                (Term _, LambdaTerm lambda) =>
                    this.UnifyLambda(lambda, term1),
                (LambdaTerm lambda, Term _) =>
                    this.UnifyLambda(lambda, term2),

                _ => this.RecordError(
                    $"Cannot unify: between \"{term1.ReadableString}\" and \"{term2.ReadableString}\"",
                    term1,
                    term2)
            };

            return result;
        }

#line hidden
        Term Term.IInferringContext.Unify(Term term1, Term term2) =>
            this.Unify(term1, term2);
#line default
    }
}
