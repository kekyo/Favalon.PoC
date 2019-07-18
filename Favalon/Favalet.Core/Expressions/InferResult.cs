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

using System.ComponentModel;

namespace Favalet.Expressions
{
    public struct InferResult<TExpression>
        where TExpression : Expression
    {
        public readonly TExpression Expression;
        public readonly InferErrorInformation[] ErrorInformations;

        private InferResult(TExpression expression, InferErrorInformation[] errorInformations)
        {
            this.Expression = expression;
            this.ErrorInformations = errorInformations;
        }

        public static InferResult<TExpression> Create(TExpression expression, InferErrorInformation[] errorInformations) =>
            new InferResult<TExpression>(expression, errorInformations);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out TExpression? expression, out InferErrorInformation[] errorInformations)
        {
            expression = this.Expression;
            errorInformations = this.ErrorInformations;
        }
    }
}
