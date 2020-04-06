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

using Favalet.Lexers.Runners;
using Favalet.Tokens;
using System;

namespace Favalet.Lexers
{
    internal sealed class ObservableStringLexer : ObservableLexer<string>
    {
        public ObservableStringLexer(IObservable<string> parent) :
            base(parent)
        { }

        protected override void OnValueReceived(string line)
        {
            for (var index = 0; index < line.Length; index++)
            {
                switch (runner.Run(context, line[index]))
                {
                    case LexRunnerResult(LexRunner next, Token token0, Token token1):
                        this.SendValues(token0, token1);
                        runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, Token token, _):
                        this.SendValues(token);
                        runner = next;
                        break;
                    case LexRunnerResult(LexRunner next, _, _):
                        runner = next;
                        break;
                }
            }
        }
    }
}
