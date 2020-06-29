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
using Favalet.Expressions.Comparer;
using Favalet.Lexers;
using Favalet.Parsers;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using static Favalet.Expressions.ExpressionFactory;
using static Favalet.Expressions.CLRExpressionFactory;
using Favalet.Internal;

namespace Favalet
{
    [TestFixture]
    public sealed class InferrerTest
    {
        private static TypeEnvironment Create() =>
            TypeEnvironment.Create(CLRTypeContextFeatures.Instance, 100).
            MutableBindTypes(typeof(object).Assembly).
            MutableBindTypes(typeof(Uri).Assembly).
            MutableBindTypes(typeof(Enumerable).Assembly);

        private static readonly Func<string, TypeEnvironment, ValueTask<IExpression[]>>[] Parsers =
            new[]
            {
                new Func<string, TypeEnvironment, ValueTask<IExpression[]>>((text, environment) =>
                    new ValueTask<IExpression[]>(
                        environment.Infer(Lexer.Tokenize(text).Parse(environment.Features)).
                        ToArray())),
                //new Func<string, TypeEnvironment, ValueTask<IExpression[]>>(async (text, environment) =>
                //    await environment.Infer(Lexer.Tokenize(text.ToObservable()).Parse(environment.Features)).Memoize()),
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

        [TestCaseSource("Parsers")]
        public async Task LookupStaticOverloadedMethod(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            // Activator.CreateInstance(typeof(int))
            var text = "System.Activator.CreateInstance System.Int32";
            var environment = Create();
            var actual = await run(text, environment);

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Method(typeof(Activator), "CreateInstance", typeof(Type)),
                        Constant(typeof(int))),
                },
                actual);
        }

        [TestCaseSource("Parsers")]
        public async Task LookupManyStaticOverloadedMethod1(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            // Convert.ToString(123)
            var text = "System.Convert.ToString 123";
            var environment = Create();
            var actual = await run(text, environment);

            var methods = new[]
                {
                    Method(typeof(Convert), "ToString", typeof(int)),
                    Method(typeof(Convert), "ToString", typeof(long)),
                    Method(typeof(Convert), "ToString", typeof(object)),
                    Method(typeof(Convert), "ToString", typeof(float)),
                    Method(typeof(Convert), "ToString", typeof(double)),
                }.OrderBy(m => m, ExpressionComparer.Instance).
                ToArray();

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Overload(methods)!,
                        Constant(123)),
                },
                actual);
        }

        [TestCaseSource("Parsers")]
        public async Task LookupManyStaticOverloadedMethod2(Func<string, TypeEnvironment, ValueTask<IExpression[]>> run)
        {
            // Convert.ToString(123)
            var text = "System.Convert.ToInt32 \"123\"";
            var environment = Create();
            var actual = await run(text, environment);

            var methods = new[]
                {
                    Method(typeof(Convert), "ToInt32", typeof(string)),
                    Method(typeof(Convert), "ToInt32", typeof(object)),
                }.OrderBy(m => m, ExpressionComparer.Instance).
                ToArray();

            Assert.AreEqual(
                new IExpression[]
                {
                    Apply(
                        Overload(methods)!,
                        Constant("123")),
                },
                actual);
        }

        //[Test]
        //public void ExpressionComparerTest()
        //{
        //    var b = Method(typeof(Convert), "ToInt32", typeof(byte));
        //    var o = Method(typeof(Convert), "ToInt32", typeof(object));

        //    var r = ReflectionUtilities.Compare(b.Method, o.Method);
        //    var rr = ReflectionUtilities.Compare(o.Method, b.Method);

        //    var r1 = ((IComparable<IExpression>)(b)).CompareTo(o);
        //    var rr1 = ((IComparable<IExpression>)(o)).CompareTo(b);

        //    var a2 = new[] { b, o };
        //    Array.Sort(a2, (i0, i1) => ((IComparable<IExpression>)(i0)).CompareTo(i1));

        //    var a3 = new[] { o, b };
        //    Array.Sort(a3, (i0, i1) => ((IComparable<IExpression>)(i0)).CompareTo(i1));

        //    var aa = new[] { 1, 3, 5, 4, 2 };
        //    Array.Sort(aa, (i0, i1) => (i0, i1) switch
        //        {
        //            _ when i0 < i1 => -1,
        //            _ when i0 > i1 => 1,
        //            _ => 0
        //        });


        //    var expected =
        //        new IExpression[]
        //        {
        //            Method(typeof(Convert), "ToInt32", typeof(byte)),
        //            Method(typeof(Convert), "ToInt32", typeof(object)),
        //        }.OrderBy(m => m, ExpressionComparer.Instance).ToArray();
        //}
    }
}
