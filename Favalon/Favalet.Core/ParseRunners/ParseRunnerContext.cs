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
using Favalet.Tokens;
using System.Diagnostics;

namespace Favalet.ParseRunners
{
    internal sealed class ParseRunnerContext
    {
        public readonly IExpressionFactory Factory;
        private NumericalSignToken? preSignToken;

        private ParseRunnerContext(IExpressionFactory factory) =>
            this.Factory = factory;

        public Expression? Expression { get; private set; }

        public Token? LastToken { get; private set; }

        public void Combine(Expression expression)
        {
            if (this.Expression != null)
            {
                this.Expression = this.Factory.Apply(this.Expression, expression);
            }
            else
            {
                this.Expression = expression;
            }
        }

        public void SetLastToken(Token token) =>
            this.LastToken = token;

        public void SetPreSignToken(NumericalSignToken token)
        {
            Debug.Assert(this.preSignToken == null);
            this.preSignToken = token;
        }

        public NumericalSignToken GetAndClearPreSignToken()
        {
            Debug.Assert(this.preSignToken != null);

            var preSignToken = this.preSignToken;
            this.preSignToken = null;
            return preSignToken!;
        }

        public static ParseRunnerContext Create(IExpressionFactory factory) =>
            new ParseRunnerContext(factory);
    }
}
