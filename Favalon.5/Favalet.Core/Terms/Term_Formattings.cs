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

using Favalet.Terms.Basis;
using Favalet.Terms.Specialized;
using System.Collections.Generic;

namespace Favalet.Terms
{
    partial class Term
    {
        protected struct FormattedString
        {
            public readonly string Formatted;
            public readonly bool RequiredEnclosingParentheses;

            private FormattedString(string formatted, bool requiredEnclosingParentheses)
            {
                this.Formatted = formatted;
                this.RequiredEnclosingParentheses = requiredEnclosingParentheses;
            }

            public static implicit operator FormattedString(string formatted) =>
                new FormattedString(formatted, false);

            public static FormattedString RequiredEnclosing(string formatted) =>
                new FormattedString(formatted, true);
        }

        protected sealed class FormatContext
        {
            private readonly SortedDictionary<PlaceholderTerm, int> freeVariables;

            private FormatContext(
                FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
                SortedDictionary<PlaceholderTerm, int> freeVariables)
            {
                this.FormatAnnotation = formatAnnotation;
                this.FormatNaming = formatNaming;
                this.FormatOperator = formatOperator;
                this.freeVariables = freeVariables;
            }

            public readonly FormatAnnotations FormatAnnotation;
            public readonly FormatNamings FormatNaming;
            public readonly FormatOperators FormatOperator;

            internal FormatContext(FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator) :
                this(formatAnnotation, formatNaming, formatOperator, new SortedDictionary<PlaceholderTerm, int>())
            {
            }

            public FormatContext NewDerived(
                FormatAnnotations? formatAnnotation, FormatNamings? formatNaming, FormatOperators? formatOperator) =>
                new FormatContext(
                    formatAnnotation ?? this.FormatAnnotation,
                    formatNaming ?? this.FormatNaming,
                    formatOperator ?? this.FormatOperator,
                    freeVariables);

            internal int GetAdjustedIndex(PlaceholderTerm freeVariable)
            {
                if (!freeVariables.TryGetValue(freeVariable, out var index))
                {
                    index = freeVariables.Count;
                    freeVariables.Add(freeVariable, index);
                }
                return index;
            }
        }

        private string FormatReadableString(FormatContext context, bool encloseParenthesesIfRequired)
        {
            var result = this.FormatReadableString(context);
            return (encloseParenthesesIfRequired && result.RequiredEnclosingParentheses) ?
                $"({result.Formatted})" :
                result.Formatted;
        }

        private static bool IsRequiredAnnotation(FormatContext context, Term term) =>
            (context.FormatAnnotation, term, term.HigherOrder) switch
            {
                (_, _, null) => false,
                (_, _, Rank3Term _) => false,                                                         // "...:#" => "..."
                (_, TypeTerm _, KindTerm _) => false,                                                 // "?:*" => "?"
                (_, LambdaTerm(TypeTerm _, TypeTerm _), KindTerm _) => false,                         // "(? -> ?):*" => "(? -> ?)"
                (_, UnspecifiedTerm _, UnspecifiedTerm _) => false,                                   // "_:_" => "_" 
                (_, Term _, LambdaTerm(UnspecifiedTerm _, Term e)) => IsRequiredAnnotation(context, e),         // "...":(_ -> _) => "..." 
                (_, Term _, LambdaTerm(Term p, UnspecifiedTerm _)) => IsRequiredAnnotation(context, p),         // "...":(_ -> _) => "..." 
                (_, LambdaTerm(Term p, Term e), Term _) => IsRequiredAnnotation(context, p) || IsRequiredAnnotation(context, e),    // "(_ -> _):..." => "(_ -> _)"
                (FormatAnnotations.Always, _, Term _) => true,
                (FormatAnnotations.Without, _, Term _) => false,
                (FormatAnnotations.Annotated, _, _) => !(term.HigherOrder is UnspecifiedTerm),
                (_ , _, UnspecifiedTerm _) => false,
                (_, _, TypeTerm type) => !type.Equals(TypeTerm.Instance),
                _ => true
            };

        protected static string FormatReadableString(FormatContext context, Term term, bool encloseParenthesesIfRequired)
        {
            if (IsRequiredAnnotation(context, term))
            {
                var variable = term.FormatReadableString(context, true);
                var annotation = FormatReadableString(
                    (context.FormatAnnotation >= FormatAnnotations.Annotated) ?
                        context :
                        context.NewDerived(FormatAnnotations.Without, null, null),
                    term.HigherOrder,
                    true);
                return $"{variable}:{annotation}";
            }
            else
            {
                return term.FormatReadableString(context, encloseParenthesesIfRequired);
            }
        }
    }
}
