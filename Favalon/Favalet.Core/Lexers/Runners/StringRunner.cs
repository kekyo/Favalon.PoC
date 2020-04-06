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

using Favalet.Internal;
using Favalet.Tokens;
using System;

namespace Favalet.Lexers.Runners
{
    internal sealed class StringRunner : LexRunner
    {
        private StringRunner()
        { }

        private static StringToken InternalFinish(LexRunnerContext context)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();

            // TODO: interpret escape sequences.

            return new StringToken(token);
        }

        public override LexRunnerResult Run(LexRunnerContext context, char ch)
        {
            if (StringUtilities.IsDoubleQuote(ch))
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context));
            }
            else
            {
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Empty(this);
            }
        }

        public override LexRunnerResult Finish(LexRunnerContext context) =>
            throw new InvalidOperationException("Couldn't find a string quote.");

        public static readonly LexRunner Instance = new StringRunner();
    }
}
