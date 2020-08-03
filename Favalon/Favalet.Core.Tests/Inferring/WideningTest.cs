﻿using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class WideningTest
    {
        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!ExpressionAssert.Equals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringTypes.Readable),
                    expected.GetPrettyString(PrettyStringTypes.Readable),
                    actual.GetPrettyString(PrettyStringTypes.Readable));
            }
        }

        [Test]
        public void WideningInLambdaBody()
        {
            var environment = CLREnvironment();

            // a:int -> a:object
            var expression =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<object>()));

            var actual = environment.Infer(expression);

            // a:int -> a:int
            var expected =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
