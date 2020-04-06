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

using Favalet.Expressions;

namespace Favalet.Contexts
{
    public struct BoundInformation
    {
        public readonly IExpression Expression;
        public readonly BoundTermAttributes Attributes;
        public readonly BoundTermPrecedences Precedence;

        public BoundInformation(
            IExpression expression,
            BoundTermAttributes attributes,
            BoundTermPrecedences precedence)
        {
            this.Expression = expression;
            this.Attributes = attributes;
            this.Precedence = precedence;
        }

        public override string ToString() =>
            $"{this.Expression}: {this.Attributes}, {this.Precedence}";
    }
}
