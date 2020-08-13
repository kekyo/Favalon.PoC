using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class IdentityTest
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
        public void FromVariable1()
        {
            var environment = CLREnvironment();

            environment.MutableBind(
                "true",
                Constant(true));

            // true
            var expression =
                Variable("true");

            var actual = environment.Infer(expression);

            // true:bool
            var expected =
                Variable("true", Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = CLREnvironment();

            environment.MutableBind(
                "true",
                Constant(true));
            environment.MutableBind(
                "false",
                Constant(false));

            // true && false
            var expression =
                And(
                    Variable("true"),
                    Variable("false"));

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Variable("true", Type<bool>()),
                    Variable("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }
 
        [Test]
        public void Placeholder1()
        {
            var environment = CLREnvironment();

            // a:int -> b:object
            var expression =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<object>()));

            var actual = environment.Infer(expression);

            // a:object -> b:object
            var expected =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a", Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
