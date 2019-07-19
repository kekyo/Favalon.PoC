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

namespace Favalet.Terms.Basis
{
    public sealed class BindTerm : Term
    {
        internal BindTerm(BoundVariableTerm bound, Term term, bool recursiveBind, Term higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Bound = bound;
            this.Term = term;
            this.RecursiveBind = recursiveBind;
        }

        public new readonly BoundVariableTerm Bound;
        public readonly Term Term;
        public new readonly bool RecursiveBind;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var rec = this.RecursiveBind ? "rec " : string.Empty;
            return FormattedString.RequiredEnclosing(
                $"{rec}{FormatReadableString(context, this.Bound, true)} = {FormatReadableString(context, this.Term, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Term VisitInferring(IInferringContext context, Term higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            if (this.RecursiveBind)
            {
                var bound = context.Visit(this.Bound, higherOrder);
                var term = context.Visit(this.Term, bound.HigherOrder);

                return new BindTerm(bound, term, true, term.HigherOrder, this.TextRange);
            }
            else
            {
                var term = context.Visit(this.Term, higherOrder);
                var bound = context.Visit(this.Bound, term.HigherOrder);

                return new BindTerm(bound, term, false, bound.HigherOrder, this.TextRange);
            }
        }

        protected override Term VisitResolving(IResolvingContext context)
        {
            var bound = context.Visit(this.Bound);
            var term = context.Visit(this.Term);
            var higherOrder = context.Visit(this.HigherOrder);

            return new BindTerm(bound, term, this.RecursiveBind, higherOrder, this.TextRange);
        }
    }
}
