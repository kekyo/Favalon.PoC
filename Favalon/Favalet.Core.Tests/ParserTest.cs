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
using Favalet.Lexers;
using Favalet.Parsers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using static Favalet.Expressions.ExpressionFactory;
using static Favalet.Expressions.CLRExpressionFactory;

namespace Favalet
{
    [TestFixture]
    public sealed class ParserTest
    {
        private static IExpression[] Run(string text) =>
            Lexer.Tokenize(text).Parse(CLRExpressionFactory.Instance).ToArray();

        private static readonly Func<string, ValueTask<IExpression[]>>[] Parsers =
            new[]
            {
                new Func<string, ValueTask<IExpression[]>>(text => new ValueTask<IExpression[]>(Lexer.Tokenize(text).Parse(CLRExpressionFactory.Instance).ToArray())),
                new Func<string, ValueTask<IExpression[]>>(async text => await Lexer.Tokenize(text.ToObservable()).Parse(CLRExpressionFactory.Instance).ToArray()),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("Parsers")]
        public async Task ParseArticleSampleCode(Func<string, ValueTask<IExpression[]>> run)
        {
            var text = "echo \"abc def ghi\" | wc";
            var actual = await run(text);

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Apply(
                            Apply(
                                Identity("echo"),
                                Constant("abc def ghi")),
                            Identity("|")),
                        Identity("wc")),
                },
                actual);
        }
    }
}
