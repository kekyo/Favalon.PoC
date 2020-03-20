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
    public sealed class InferrerTest
    {
        private static TypeEnvironment Create() =>
            TypeEnvironment.Create(CLRExpressionFactory.Instance, 100).
            MutableBindTypes(typeof(object).Assembly).
            MutableBindTypes(typeof(Uri).Assembly).
            MutableBindTypes(typeof(Enumerable).Assembly);

        private static readonly Func<string, TypeEnvironment, ValueTask<IExpression[]>>[] Parsers =
            new[]
            {
                new Func<string, TypeEnvironment, ValueTask<IExpression[]>>((text, environment) =>
                    new ValueTask<IExpression[]>(
                        environment.Infer(Lexer.Tokenize(text).Parse(CLRExpressionFactory.Instance)).
                        ToArray())),
                //new Func<string, TypeEnvironment, ValueTask<IExpression[]>>(async (text, environment) =>
                //    await environment.Infer(Lexer.Tokenize(text.ToObservable()).Parse(CLRExpressionFactory.Instance)).Memoize()),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("Parsers")]
        public async Task LookupStaticMethod(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            var text = "System.Int32.Parse \"123\"";
            var environment = Create();
            var actual = await run(text, environment);

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Method<int>("Parse", typeof(string)),
                        Constant("123")),
                },
                actual);
        }

        [TestCaseSource("Parsers")]
        public async Task LookupType(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            var text = "System.Int32";
            var environment = Create();
            var actual = await run(text, environment);

            Assert.AreEqual(
                new IExpression[]
                {
                    Type<int>()
                },
                actual);
        }

        [TestCaseSource("Parsers")]
        public async Task LookupConstructor(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            var text = "System.Uri \"https://example.com/\"";
            var environment = Create();
            var actual = await run(text, environment);

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Constructor<Uri>(typeof(string)),
                        Constant("https://example.com/")),
                },
                actual);
        }
    }
}
