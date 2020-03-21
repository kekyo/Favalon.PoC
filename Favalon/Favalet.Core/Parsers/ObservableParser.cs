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
using Favalet.Internal;
using Favalet.Parsers.Runners;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    internal sealed class ObservableParser : ObservableObserver<IExpression, Token>
    {
        private readonly ParseRunnerContext context;
        private ParseRunner runner = WaitingRunner.Instance;
#if DEBUG
        private int index = 0;
#endif

        public ObservableParser(IObservable<Token> parent, ITypeContextFeatures features) :
            base(parent) =>
            this.context = ParseRunnerContext.Create(features);

        protected override void OnValueReceived(Token token)
        {
#if DEBUG
            if (index == Parser.BreakIndex) Debugger.Break();
            index++;
#endif
            runner = runner.Run(context, token);

            Debug.WriteLine($"{index - 1}: '{token}': {context.Expression}");

            context.SetLastToken(token);
        }

        protected override void OnFinalize()
        {
            // Contained final result
            if (context.Expression is IExpression finalExpression)
            {
                this.SendAndFinish(finalExpression);
            }
            else
            {
                this.Finish();
            }
        }
    }
}
