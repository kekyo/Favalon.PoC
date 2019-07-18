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
using System.ComponentModel;

namespace Favalet.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        internal ApplyExpression(Expression function, Expression argument, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public readonly Expression Function;
        public readonly Expression Argument;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Argument, true)}");

        protected override Expression VisitInferring(IInferringContext context, Expression higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            var visitedArgument = context.Visit(this.Argument, UnspecifiedExpression.Instance);

            var functionHigherOrder = LambdaExpression.Create(
                visitedArgument.HigherOrder, higherOrder, true, this.TextRange);
            var visitedFunction = context.Visit(this.Function, functionHigherOrder);

            return new ApplyExpression(visitedFunction, visitedArgument, higherOrder, this.TextRange);
        }

        protected override Expression VisitResolving(IResolvingContext context)
        {
            var argument = context.Visit( this.Argument);
            var function = context.Visit(this.Function);
            var higherOrder = context.Visit(this.HigherOrder);

            return new ApplyExpression(function, argument, higherOrder, this.TextRange);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Expression function, out Expression argument)
        {
            function = this.Function;
            argument = this.Argument;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Expression function, out Expression argument, out Expression higherOrder)
        {
            function = this.Function;
            argument = this.Argument;
            higherOrder = this.HigherOrder;
        }
    }
}
