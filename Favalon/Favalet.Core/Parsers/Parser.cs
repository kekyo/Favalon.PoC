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
using Favalet.Parsers.Runners;
using Favalet.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet.Parsers
{
    public static class Parser
    {
#if DEBUG
        public static int BreakIndex = -1;
#endif

        public static IEnumerable<IExpression> Parse(this IEnumerable<Token> tokens) =>
            Parse(tokens, TypeContextFeatures.Instance);

        public static IEnumerable<IExpression> Parse(
            this IEnumerable<Token> tokens, ITypeContextFeatures features)
        {
            var context = ParseRunnerContext.Create(features);
            var runner = WaitingRunner.Instance;

#if DEBUG
            var index = 0;
#endif
            foreach (var token in tokens)
            {
#if DEBUG
                if (index == BreakIndex) Debugger.Break();
                index++;
#endif
                runner = runner.Run(context, token);
#if DEBUG
                Debug.WriteLine($"Parse [{index - 1}]: Token={token}, Expression={context.Expression}");
#endif
                context.SetLastToken(token);
            }

            // Contained final result
            if (context.Expression is IExpression finalExpression)
            {
                Debug.WriteLine($"Parse [F]: Expression={finalExpression}");

                yield return finalExpression;
            }
        }

#if !NET35
        public static IObservable<IExpression> Parse(this IObservable<Token> tokens) =>
            new ObservableParser(tokens, TypeContextFeatures.Instance);

        public static IObservable<IExpression> Parse(
            this IObservable<Token> tokens, ITypeContextFeatures features) =>
            new ObservableParser(tokens, features);
#endif
    }
}
