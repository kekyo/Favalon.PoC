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

using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Tokens;
using System.Diagnostics;

namespace Favalet.Parsers.Runners
{
    internal sealed class ParseRunnerContext
    {
        public readonly ITypeContextFeatures Features;
        private NumericalSignToken? preSignToken;

        private ParseRunnerContext(ITypeContextFeatures features) =>
            this.Features = features;

        public IExpression? Expression { get; private set; }

        public Token? LastToken { get; private set; }

        public void Combine(IExpression expression)
        {
            if (this.Expression != null)
            {
                this.Expression = this.Features.CreateApply(this.Expression, expression);
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

        public static ParseRunnerContext Create(ITypeContextFeatures features) =>
            new ParseRunnerContext(features);
    }
}
