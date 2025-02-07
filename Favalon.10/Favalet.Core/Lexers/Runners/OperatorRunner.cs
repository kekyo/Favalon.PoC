﻿////////////////////////////////////////////////////////////////////////////
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
    internal sealed class OperatorRunner : LexRunner
    {
        private OperatorRunner()
        { }

        private static Token InternalFinish(LexRunnerContext context, bool forceIdentity)
        {
            var token = context.TokenBuffer.ToString();
            context.TokenBuffer.Clear();
            if (!forceIdentity && (token.Length == 1) &&
                StringUtilities.IsNumericSign(token[0]) is NumericalSignes sign)
            {
                return (sign == NumericalSignes.Plus) ?
                    TokenFactory.PlusSign() : TokenFactory.MinusSign();
            }
            else
            {
                return TokenFactory.Identity(token);
            }
        }

        public override LexRunnerResult Run(LexRunnerContext context, char ch)
        {
            if (char.IsWhiteSpace(ch))
            {
                return LexRunnerResult.Create(
                    WaitingIgnoreSpaceRunner.Instance,
                    InternalFinish(context, true),
                    WhiteSpaceToken.Instance);
            }
            else if (char.IsDigit(ch))
            {
                var token = InternalFinish(context, false);
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Create(
                    NumericRunner.Instance,
                    token);
            }
            else if (StringUtilities.IsDoubleQuote(ch))
            {
                return LexRunnerResult.Create(
                    StringRunner.Instance,
                    InternalFinish(context, true));
            }
            else if (StringUtilities.IsOpenParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    TokenFactory.Open(ch));
            }
            else if (StringUtilities.IsCloseParenthesis(ch) is ParenthesisPair)
            {
                return LexRunnerResult.Create(
                    WaitingRunner.Instance,
                    InternalFinish(context, true),
                    TokenFactory.Close(ch));
            }
            else if (StringUtilities.IsOperator(ch))
            {
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Empty(this);
            }
            else if(!char.IsControl(ch))
            {
                var token = InternalFinish(context, true);
                context.TokenBuffer.Append(ch);
                return LexRunnerResult.Create(
                    IdentityRunner.Instance,
                    token);
            }
            else
            {
                throw new InvalidOperationException(ch.ToString());
            }
        }

        public override LexRunnerResult Finish(LexRunnerContext context) =>
            LexRunnerResult.Create(WaitingRunner.Instance, InternalFinish(context, true));

        public static readonly LexRunner Instance = new OperatorRunner();
    }
}
