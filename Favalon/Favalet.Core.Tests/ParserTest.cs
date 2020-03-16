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
using NUnit.Framework;
using System;
using System.Linq;

using static Favalet.Expressions.ExpressionFactory;
using static Favalet.Expressions.CLRExpressionFactory;

namespace Favalet
{
    [TestFixture]
    public sealed class ParserTest
    {
        private static readonly Func<string, IExpression[]>[] ParserRunners =
            new[]
            {
                new Func<string, IExpression[]>(text =>
                Lexer.EnumerableTokens(text).Parse(CLRExpressionFactory.Instance).ToArray()),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("ParserRunners")]
        public void ParseIdentityTokens(Func<string, IExpression[]> run)
        {
            var text = "echo \"abc def ghi\" | wc";
            var actual = run(text);

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
