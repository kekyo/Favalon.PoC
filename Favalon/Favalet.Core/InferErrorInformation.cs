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
    public sealed class InferErrorInformation : ErrorInformation
    {
        private InferErrorInformation(string details, Expression primaryExpression, Expression[] expressions) :
            base(details)
        {
            this.PrimaryExpression = primaryExpression;
            this.Expressions = expressions;
        }

        public readonly Expression PrimaryExpression;
        public readonly Expression[] Expressions;

        public override TextRange TextRange =>
            this.PrimaryExpression.TextRange;

        public static InferErrorInformation Create(string details, Expression primaryExpression, params Expression[] expressions) =>
            new InferErrorInformation(details, primaryExpression, expressions);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Deconstruct(out string details, out Expression primaryExpression, out Expression[] expressions)
        {
            details = this.Details;
            primaryExpression = this.PrimaryExpression;
            expressions = this.Expressions;
        }
    }
}
