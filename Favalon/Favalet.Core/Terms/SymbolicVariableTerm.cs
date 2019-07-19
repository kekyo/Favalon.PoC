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
using System;

namespace Favalet.Terms
{
    public abstract class SymbolicVariableTerm :
        VariableTerm, IEquatable<SymbolicVariableTerm?>
    {
        protected SymbolicVariableTerm(string name, Term higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Name = name;

        public readonly string Name;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            this.Name;

        protected abstract Term CreateTermOnVisitInferring(Term higherOrder);

        private static Term VisitNonPlaceholder(IInferringContext context, Term term) =>
            term is PlaceholderTerm ?
                term :
                context.Visit(term, UnspecifiedTerm.Instance);

        protected virtual Term VisitInferringOnBoundTermNotFound(
            IInferringContext context, Term higherOrderHint)
        {
            Term higherOrder;

            switch (this.HigherOrder, higherOrderHint)
            {
                case (UnspecifiedTerm _, UnspecifiedTerm _):
                    higherOrder = context.CreatePlaceholder(UnspecifiedTerm.Instance, this.TextRange);
                    break;
                case (UnspecifiedTerm _, Term _):
                    higherOrder = VisitNonPlaceholder(context, higherOrderHint);
                    break;
                case (Term _, UnspecifiedTerm _):
                    higherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                    break;
                case (Term _, Term _):
                    {
                        var visitedHigherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                        var visitedHigherOrderHint = VisitNonPlaceholder(context, higherOrderHint);
                        higherOrder = context.Unify(visitedHigherOrder, visitedHigherOrderHint);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var result = this.CreateTermOnVisitInferring(higherOrder);
            context.Memoize(this, result);

            return result;
        }

        protected override Term VisitInferring(
            IInferringContext context, Term higherOrderHint)
        {
            if (context.Lookup(this) is Term bound)
            {
                Term higherOrder;

                switch (this.HigherOrder, higherOrderHint, bound.HigherOrder)
                {
                    case (UnspecifiedTerm _, UnspecifiedTerm _, UnspecifiedTerm _):
                        higherOrder = context.CreatePlaceholder(UnspecifiedTerm.Instance, this.TextRange);
                        break;
                    case (UnspecifiedTerm _, UnspecifiedTerm _, Term _):
                        higherOrder = VisitNonPlaceholder(context, bound.HigherOrder);
                        break;
                    case (UnspecifiedTerm _, Term _, UnspecifiedTerm _):
                        higherOrder = VisitNonPlaceholder(context, higherOrderHint);
                        break;
                    case (Term _, UnspecifiedTerm _, UnspecifiedTerm _):
                        higherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                        break;
                    case (UnspecifiedTerm _, Term _, Term _):
                        {
                            var visitedHigherOrderHint = VisitNonPlaceholder(context, higherOrderHint);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(context, bound.HigherOrder);
                            higherOrder = context.Unify(visitedHigherOrderHint, visitedBoundHigherOrder);
                        }
                        break;
                    case (Term _, Term _, UnspecifiedTerm _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                            var visitedHigherOrderHint = VisitNonPlaceholder(context, higherOrderHint);
                            higherOrder = context.Unify(visitedHigherOrder, visitedHigherOrderHint);
                        }
                        break;
                    case (Term _, UnspecifiedTerm _, Term _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(context, bound.HigherOrder);
                            higherOrder = context.Unify(visitedHigherOrder, visitedBoundHigherOrder);
                        }
                        break;
                    case (Term _, Term _, Term _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(context, this.HigherOrder);
                            var visitedHigherOrderHint = VisitNonPlaceholder(context, higherOrderHint);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(context, bound.HigherOrder);
                            higherOrder = context.Unify(visitedHigherOrderHint, visitedBoundHigherOrder);
                            higherOrder = context.Unify(visitedHigherOrder, higherOrder);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                return this.CreateTermOnVisitInferring(higherOrder);
            }
            else
            {
                return this.VisitInferringOnBoundTermNotFound(context, higherOrderHint);
            }
        }

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(SymbolicVariableTerm? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as SymbolicVariableTerm);
    }
}
