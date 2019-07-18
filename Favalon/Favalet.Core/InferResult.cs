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

namespace Favalet
{
    public struct InferResult
    {
        public readonly Expression Expression;
        public readonly InferErrorInformation[] ErrorInformations;

        private InferResult(Expression expression, InferErrorInformation[] errorInformations)
        {
            this.Expression = expression;
            this.ErrorInformations = errorInformations;
        }

        public static InferResult Create(Expression expression, InferErrorInformation[] errorInformations) =>
            new InferResult(expression, errorInformations);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out Expression? expression, out InferErrorInformation[] errorInformations)
        {
            expression = this.Expression;
            errorInformations = this.ErrorInformations;
        }
    }
}
