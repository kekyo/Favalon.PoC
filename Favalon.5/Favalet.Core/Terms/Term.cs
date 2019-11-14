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

using System.Diagnostics;

namespace Favalet.Terms
{
    public interface ITraversableTerm
    {
        Term[] Children { get; }
    }

    [DebuggerDisplay("{TermName}: {AnnotatedReadableString}")]
    public abstract partial class Term
    {
        protected Term(Term higherOrder, TextRange textRange)
        {
            this.HigherOrder = higherOrder;
            this.TextRange = textRange;
        }

        public readonly Term HigherOrder;
        public readonly TextRange TextRange;

        protected abstract Term VisitInferring(IInferringContext context, Term higherOrderHint);
        protected abstract Term VisitResolving(IResolvingContext context);

        protected abstract FormattedString FormatReadableString(FormatContext context);

        public string ReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Standard, FormatNamings.Friendly, FormatOperators.Standard),
                this, false);
        public string AnnotatedReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Annotated, FormatNamings.Friendly, FormatOperators.Standard),
                this, false);
        public string StrictReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Always, FormatNamings.Standard, FormatOperators.Standard),
                this, false);

        public string FormatReadableString(
            FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
            bool encloseParenthesesIfRequired = false) =>
            FormatReadableString(new FormatContext(
                formatAnnotation, formatNaming, formatOperator), this, encloseParenthesesIfRequired);

        public string TermName =>
            this.GetType().Name.Replace("Term", string.Empty);

        public override string ToString() =>
            $"{this.TextRange}: {this.TermName}: {FormatReadableString(new FormatContext(FormatAnnotations.Standard, FormatNamings.Standard, FormatOperators.Standard), this, false)}";

        public void Deconstruct(out Term higherOrder, out TextRange textRange)
        {
            higherOrder = this.HigherOrder;
            textRange = this.TextRange;
        }
    }
}
