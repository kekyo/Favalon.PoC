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
using System.ComponentModel;
using System.Diagnostics;

namespace Favalet.Terms.Basis
{
    public sealed class LambdaTerm : ValueTerm
    {
        private LambdaTerm(Term parameter, Term term, Term higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Parameter = parameter;
            this.Term = term;
        }

        public readonly Term Parameter;
        public readonly Term Term;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var arrow = (context.FormatOperator == FormatOperators.Fancy) ? "→" : "->";
            return FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Parameter, true)} {arrow} {FormatReadableString(context, this.Term, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Term VisitInferring(IInferringContext context, Term higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            var visitedParameter = higherOrder switch
            {
                LambdaTerm(Term parameter, Term _) => context.Visit(this.Parameter, parameter),
                _ => context.Visit(this.Parameter, UnspecifiedTerm.Instance),
            };

            var visitedTerm = higherOrder switch
            {
                LambdaTerm(Term _, Term term) => context.Visit(this.Term, term),
                _ => context.Visit(this.Term, UnspecifiedTerm.Instance),
            };

            var visitedHigherOrder = new LambdaTerm(
                visitedParameter.HigherOrder, visitedTerm.HigherOrder, UnspecifiedTerm.Instance, this.TextRange);
            if (!(higherOrder is LambdaTerm))
            {
                context.Unify(higherOrder, visitedHigherOrder);
            }

            return new LambdaTerm(visitedParameter, visitedTerm, higherOrder, this.TextRange);
        }

        protected override Term VisitResolving(IResolvingContext context)
        {
            var parameter = context.Visit(this.Parameter);
            var term = context.Visit(this.Term);
            var higherOrder = context.Visit(this.HigherOrder);

            return new LambdaTerm(parameter, term, higherOrder, this.TextRange);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Term parameter, out Term term)
        {
            parameter = this.Parameter;
            term = this.Term;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Term parameter, out Term term, out Term higherOrder)
        {
            parameter = this.Parameter;
            term = this.Term;
            higherOrder = this.HigherOrder;
        }

        internal static LambdaTerm Create(
            Term parameter, Term term, bool isRecursive, TextRange textRange)
        {
            Debug.Assert((parameter is Term) && (term is Term));

            return (parameter, term) switch
            {
                (UnspecifiedTerm _, UnspecifiedTerm _) =>
                    new LambdaTerm(parameter, term, UnspecifiedTerm.Instance, textRange),
                (Term _, UnspecifiedTerm _) =>
                    new LambdaTerm(parameter, UnspecifiedTerm.Instance, UnspecifiedTerm.Instance, textRange),
                (UnspecifiedTerm _, Term _) =>
                    new LambdaTerm(UnspecifiedTerm.Instance, term, UnspecifiedTerm.Instance, textRange),
                (KindTerm _, KindTerm _) =>
                    new LambdaTerm(parameter, term, Rank3Term.Instance, textRange),
                _ => new LambdaTerm(parameter, term, isRecursive ?
                    (Term)Create(parameter.HigherOrder, term.HigherOrder, true, textRange) :
                    UnspecifiedTerm.Instance, textRange),
            };
        }

        internal static LambdaTerm CreateWithPlaceholder(
            IInferringContext context, Term parameter, Term term, bool isRecursive, TextRange textRange)
        {
            Debug.Assert((parameter is Term) && (term is Term));

            return (parameter, term) switch
            {
                (UnspecifiedTerm _, UnspecifiedTerm _) =>
                    new LambdaTerm(parameter, term, UnspecifiedTerm.Instance, textRange),
                (Term _, UnspecifiedTerm _) =>
                    new LambdaTerm(
                        parameter,
                        context.CreatePlaceholder(UnspecifiedTerm.Instance, textRange),
                        isRecursive ?
                            (Term)CreateWithPlaceholder(context, parameter.HigherOrder, UnspecifiedTerm.Instance, true, textRange) :
                            UnspecifiedTerm.Instance, textRange),
                (UnspecifiedTerm _, Term _) =>
                    new LambdaTerm(
                        context.CreatePlaceholder(UnspecifiedTerm.Instance, textRange),
                        term,
                        isRecursive ?
                            (Term)CreateWithPlaceholder(context, UnspecifiedTerm.Instance, term.HigherOrder, true, textRange) :
                            UnspecifiedTerm.Instance, textRange),
                (KindTerm _, KindTerm _) =>
                    new LambdaTerm(parameter, term, Rank3Term.Instance, textRange),
                _ => new LambdaTerm(
                    parameter,
                    term,
                    isRecursive ?
                        (Term)CreateWithPlaceholder(context, parameter.HigherOrder, term.HigherOrder, true, textRange) :
                        UnspecifiedTerm.Instance, textRange),
            };
        }
    }
}
