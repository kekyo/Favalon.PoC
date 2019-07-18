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

namespace Favalet.Expressions
{
    public sealed class BindExpression : Expression
    {
        internal BindExpression(BoundVariableExpression bound, Expression expression, bool recursiveBind, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Bound = bound;
            this.Expression = expression;
            this.RecursiveBind = recursiveBind;
        }

        public new readonly BoundVariableExpression Bound;
        public readonly Expression Expression;
        public new readonly bool RecursiveBind;

        protected override FormattedString FormatReadableString(FormatContext context)
        {
            var rec = this.RecursiveBind ? "rec " : string.Empty;
            return FormattedString.RequiredEnclosing(
                $"{rec}{FormatReadableString(context, this.Bound, true)} = {FormatReadableString(context, this.Expression, context.FormatNaming != FormatNamings.Friendly)}");
        }

        protected override Expression VisitInferring(IInferringContext context, Expression higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            if (this.RecursiveBind)
            {
                var bound = context.Visit(this.Bound, higherOrder);
                var expression = context.Visit(this.Expression, bound.HigherOrder);

                return new BindExpression(bound, expression, true, expression.HigherOrder, this.TextRange);
            }
            else
            {
                var expression = context.Visit(this.Expression, higherOrder);
                var bound = context.Visit(this.Bound, expression.HigherOrder);

                return new BindExpression(bound, expression, false, bound.HigherOrder, this.TextRange);
            }
        }

        protected override Expression VisitResolving(IResolvingContext context)
        {
            var bound = context.Visit(this.Bound);
            var expression = context.Visit(this.Expression);
            var higherOrder = context.Visit(this.HigherOrder);

            return new BindExpression(bound, expression, this.RecursiveBind, higherOrder, this.TextRange);
        }
    }
}
