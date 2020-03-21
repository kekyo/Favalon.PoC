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

using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers.Runners
{
    internal sealed class ApplyingRunner : ParseRunner
    {
        private ApplyingRunner()
        { }

        public override ParseRunner Run(ParseRunnerContext context, Token token)
        {
            switch (token)
            {
                // 123
                case NumericToken numeric:
                    context.Combine(context.Features.CreateNumeric(numeric.Value));
                    return this;

                case NumericalSignToken numericSign:
                    // "abc -" / "123 -" ==> binary op or signed
                    if (context.LastToken is WhiteSpaceToken)
                    {
                        context.SetPreSignToken(numericSign);
                        return NumericalSignedRunner.Instance;
                    }
                    // "abc-" / "123-" / "(abc)-" ==> binary op
                    else
                    {
                        context.Combine(context.Features.CreateIdentity(numericSign.Symbol.ToString()));
                        return this;
                    }

                // "ABC"
                case StringToken str:
                    context.Combine(context.Features.CreateString(str.Value));
                    return this;

                // abc
                case IdentityToken identity:
                    context.Combine(context.Features.CreateIdentity(identity.Identity));
                    return this;

                // &
                case SymbolToken symbol:
                    context.Combine(context.Features.CreateIdentity(symbol.Symbol.ToString()));
                    return this;

                case WhiteSpaceToken _:
                    return this;

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
