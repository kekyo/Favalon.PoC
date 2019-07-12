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
    public sealed class BoundVariableExpression : SymbolicVariableExpression
    {
        internal BoundVariableExpression(string name, Expression higherOrder) :
            base(name, higherOrder)
        { }

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            this.VisitInferringImplicitVariable(
                environment,
                (name, higherOrder) => new BoundVariableExpression(name, higherOrder),
                higherOrderHint);

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var higherOrder = environment.Visit(this.HigherOrder);
            return new BoundVariableExpression(this.Name, higherOrder);
        }
    }
}
