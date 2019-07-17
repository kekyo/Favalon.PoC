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
using System;

namespace Favalet.Expressions
{
    public abstract class SymbolicVariableExpression :
        VariableExpression, IEquatable<SymbolicVariableExpression?>
    {
        protected SymbolicVariableExpression(string name, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Name = name;

        public readonly string Name;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            this.Name;

        protected abstract Expression CreateExpressionOnVisitInferring(Expression higherOrder);

        private static Expression VisitNonPlaceholder(IInferringEnvironment environment, Expression expression) =>
            expression is PlaceholderExpression ?
                expression :
                environment.Visit(expression, UnspecifiedExpression.Instance);

        protected virtual Expression VisitInferringOnBoundExpressionNotFound(
            IInferringEnvironment environment, Expression higherOrderHint)
        {
            Expression higherOrder;

            switch (this.HigherOrder, higherOrderHint)
            {
                case (UnspecifiedExpression _, UnspecifiedExpression _):
                    higherOrder = environment.CreatePlaceholder(UnspecifiedExpression.Instance, this.TextRange);
                    break;
                case (UnspecifiedExpression _, Expression _):
                    higherOrder = VisitNonPlaceholder(environment, higherOrderHint);
                    break;
                case (Expression _, UnspecifiedExpression _):
                    higherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                    break;
                case (Expression _, Expression _):
                    {
                        var visitedHigherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                        var visitedHigherOrderHint = VisitNonPlaceholder(environment, higherOrderHint);
                        higherOrder = environment.Unify(visitedHigherOrder, visitedHigherOrderHint);
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var result = this.CreateExpressionOnVisitInferring(higherOrder);
            environment.Memoize(this, result);

            return result;
        }

        protected override Expression VisitInferring(
            IInferringEnvironment environment, Expression higherOrderHint)
        {
            if (environment.Lookup(this) is Expression bound)
            {
                Expression higherOrder;

                switch (this.HigherOrder, higherOrderHint, bound.HigherOrder)
                {
                    case (UnspecifiedExpression _, UnspecifiedExpression _, UnspecifiedExpression _):
                        higherOrder = environment.CreatePlaceholder(UnspecifiedExpression.Instance, this.TextRange);
                        break;
                    case (UnspecifiedExpression _, UnspecifiedExpression _, Expression _):
                        higherOrder = VisitNonPlaceholder(environment, bound.HigherOrder);
                        break;
                    case (UnspecifiedExpression _, Expression _, UnspecifiedExpression _):
                        higherOrder = VisitNonPlaceholder(environment, higherOrderHint);
                        break;
                    case (Expression _, UnspecifiedExpression _, UnspecifiedExpression _):
                        higherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                        break;
                    case (UnspecifiedExpression _, Expression _, Expression _):
                        {
                            var visitedHigherOrderHint = VisitNonPlaceholder(environment, higherOrderHint);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(environment, bound.HigherOrder);
                            higherOrder = environment.Unify(visitedHigherOrderHint, visitedBoundHigherOrder);
                        }
                        break;
                    case (Expression _, Expression _, UnspecifiedExpression _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                            var visitedHigherOrderHint = VisitNonPlaceholder(environment, higherOrderHint);
                            higherOrder = environment.Unify(visitedHigherOrder, visitedHigherOrderHint);
                        }
                        break;
                    case (Expression _, UnspecifiedExpression _, Expression _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(environment, bound.HigherOrder);
                            higherOrder = environment.Unify(visitedHigherOrder, visitedBoundHigherOrder);
                        }
                        break;
                    case (Expression _, Expression _, Expression _):
                        {
                            var visitedHigherOrder = VisitNonPlaceholder(environment, this.HigherOrder);
                            var visitedHigherOrderHint = VisitNonPlaceholder(environment, higherOrderHint);
                            var visitedBoundHigherOrder = VisitNonPlaceholder(environment, bound.HigherOrder);
                            higherOrder = environment.Unify(visitedHigherOrderHint, visitedBoundHigherOrder);
                            higherOrder = environment.Unify(visitedHigherOrder, higherOrder);
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                return this.CreateExpressionOnVisitInferring(higherOrder);
            }
            else
            {
                return this.VisitInferringOnBoundExpressionNotFound(environment, higherOrderHint);
            }
        }

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(SymbolicVariableExpression? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as SymbolicVariableExpression);
    }
}
