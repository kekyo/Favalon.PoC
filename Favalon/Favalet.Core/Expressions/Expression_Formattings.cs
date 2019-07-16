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
using System.Collections.Generic;

namespace Favalet.Expressions
{
    partial class Expression
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
            private readonly SortedDictionary<PlaceholderExpression, int> freeVariables;

            private FormatContext(
                FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
                SortedDictionary<PlaceholderExpression, int> freeVariables)
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
                this(formatAnnotation, formatNaming, formatOperator, new SortedDictionary<PlaceholderExpression, int>())
            {
            }

            public FormatContext NewDerived(
                FormatAnnotations? formatAnnotation, FormatNamings? formatNaming, FormatOperators? formatOperator) =>
                new FormatContext(
                    formatAnnotation ?? this.FormatAnnotation,
                    formatNaming ?? this.FormatNaming,
                    formatOperator ?? this.FormatOperator,
                    freeVariables);

            internal int GetAdjustedIndex(PlaceholderExpression freeVariable)
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

        private static bool IsRequiredAnnotation(FormatContext context, Expression expression) =>
            (context.FormatAnnotation, expression, expression.HigherOrder) switch
            {
                (_, _, null) => false,
                (_, _, Rank3Expression _) => false,                                                         // "...:#" => "..."
                (_, TypeExpression _, KindExpression _) => false,                                           // "?:*" => "?"
                (_, LambdaExpression(TypeExpression _, TypeExpression _), KindExpression _) => false,       // "(? -> ?):*" => "(? -> ?)"
                (_, UnspecifiedExpression _, UnspecifiedExpression _) => false,                             // "_:_" => "_" 
                (_, Expression _, LambdaExpression(UnspecifiedExpression _, Expression e)) => IsRequiredAnnotation(context, e),         // "...":(_ -> _) => "..." 
                (_, Expression _, LambdaExpression(Expression p, UnspecifiedExpression _)) => IsRequiredAnnotation(context, p),         // "...":(_ -> _) => "..." 
                (_, LambdaExpression(Expression p, Expression e), Expression _) => IsRequiredAnnotation(context, p) || IsRequiredAnnotation(context, e),    // "(_ -> _):..." => "(_ -> _)"
                (FormatAnnotations.Always, _, Expression _) => true,
                (FormatAnnotations.Without, _, Expression _) => false,
                (FormatAnnotations.Annotated, _, _) => !(expression.HigherOrder is UnspecifiedExpression),
                (_ , _, UnspecifiedExpression _) => false,
                (_, _, TypeExpression type) => !type.Equals(TypeExpression.Instance),
                _ => true
            };

        protected static string FormatReadableString(FormatContext context, Expression expression, bool encloseParenthesesIfRequired)
        {
            if (IsRequiredAnnotation(context, expression))
            {
                var variable = expression.FormatReadableString(context, true);
                var annotation = FormatReadableString(
                    (context.FormatAnnotation >= FormatAnnotations.Annotated) ?
                        context :
                        context.NewDerived(FormatAnnotations.Without, null, null),
                    expression.HigherOrder,
                    true);
                return $"{variable}:{annotation}";
            }
            else
            {
                return expression.FormatReadableString(context, encloseParenthesesIfRequired);
            }
        }
    }
}
