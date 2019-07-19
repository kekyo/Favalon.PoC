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

namespace Favalet.Terms.Basis
{
    public sealed class ApplyTerm : Term
    {
        internal ApplyTerm(Term function, Term argument, Term higherOrder, TextRange textRange) :
            base(higherOrder, textRange)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public readonly Term Function;
        public readonly Term Argument;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Argument, true)}");

        protected override Term VisitInferring(IInferringContext context, Term higherOrderHint)
        {
            var higherOrder = context.Unify(higherOrderHint, this.HigherOrder);

            var visitedArgument = context.Visit(this.Argument, UnspecifiedTerm.Instance);

            var functionHigherOrder = LambdaTerm.Create(
                visitedArgument.HigherOrder, higherOrder, true, this.TextRange);
            var visitedFunction = context.Visit(this.Function, functionHigherOrder);

            return new ApplyTerm(visitedFunction, visitedArgument, higherOrder, this.TextRange);
        }

        protected override Term VisitResolving(IResolvingContext context)
        {
            var argument = context.Visit( this.Argument);
            var function = context.Visit(this.Function);
            var higherOrder = context.Visit(this.HigherOrder);

            return new ApplyTerm(function, argument, higherOrder, this.TextRange);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Term function, out Term argument)
        {
            function = this.Function;
            argument = this.Argument;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Term function, out Term argument, out Term higherOrder)
        {
            function = this.Function;
            argument = this.Argument;
            higherOrder = this.HigherOrder;
        }
    }
}
