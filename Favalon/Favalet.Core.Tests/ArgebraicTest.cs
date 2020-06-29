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

using System;
using System.Linq;

using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;

using static Favalet.Expressions.CLRExpressionFactory;
using static Favalet.Expressions.ExpressionFactory;

namespace Favalet
{
    [TestFixture]
    public sealed class ArgebraicTest
    {
        private static TypeEnvironment Create() =>
            TypeEnvironment.Create(CLRTypeContextFeatures.Instance, 100).
            MutableBindTypes(typeof(object).Assembly).
            MutableBindTypes(typeof(Uri).Assembly).
            MutableBindTypes(typeof(Enumerable).Assembly);

        ////////////////////////////////////////////////////

        [Test]
        public void SetTest()
        {
            var expr = Set(
                Constant(123),
                Constant("abc"),
                Type<int>());

            var environment = Create();
            var inferred = environment.Infer(expr!);
            var actual = environment.Reduce(inferred);

            Assert.AreEqual(
                expr,
                actual);
        }
    }
}
