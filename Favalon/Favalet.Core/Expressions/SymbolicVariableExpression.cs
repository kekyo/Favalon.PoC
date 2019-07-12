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

using Favalet.Expressions.Internals;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class SymbolicVariableExpression :
        VariableExpression, IEquatable<SymbolicVariableExpression?>
    {
        protected SymbolicVariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public readonly string Name;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            this.Name;

        private protected TExpression VisitInferringImplicitVariable<TExpression>(
            IInferringEnvironment environment, Func<string, Expression, TExpression> generator, Expression higherOrderHint)
            where TExpression : VariableExpression
        {
            if (environment.Lookup(this) is Expression bound)
            {
                var newHigherOrder = environment.Unify(higherOrderHint, this.HigherOrder, bound.HigherOrder);
                return generator(this.Name, newHigherOrder);
            }
            else
            {
                var newHigherOrder = environment.Unify(higherOrderHint, this.HigherOrder);
                var variable = generator(this.Name, newHigherOrder);
                environment.Memoize(this, variable);
                return variable;
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
