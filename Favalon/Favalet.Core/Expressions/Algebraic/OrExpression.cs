////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Expressions.Specialized;
using System.Collections.Generic;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression :
        IOperatorExpression
    {
    }

    public sealed class OrExpression :
        OperatorExpression<IOrExpression>, IOrExpression
    {
        private OrExpression(IExpression[] operands, IExpression higherOrder) : 
            base(operands, higherOrder)
        { }

        protected override IOrExpression Create(IExpression[] operands, IExpression higherOrder) =>
            new OrExpression(operands, higherOrder);

        public static IExpression? From(IEnumerable<IExpression> operands) =>
            From(operands, ops => new OrExpression(ops, UnspecifiedTerm.Instance));
    }
}
