using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class OverloadingTest
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
        public void OverloadingExactMatch1()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Constant(123));
            environment.MutableBind("a", Constant(123.456));

            // a:int
            var expression =
                Variable("a", Type<int>());

            var actual = environment.Infer(expression);

            // a:int
            var expected =
                Variable("a", Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }
                
        [Test]
        public void OverloadingExactMatch2()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Constant(123));
            environment.MutableBind("a", Constant(123.456));

            // a:double
            var expression =
                Variable("a", Type<double>());

            var actual = environment.Infer(expression);

            // a:double
            var expected =
                Variable("a", Type<double>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyOverloadingExactMatch1()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Constant(123));
            environment.MutableBind("a", Constant(123.456));

            // (x:int -> x) a
            var expression =
                Apply(
                    Lambda(
                        BoundVariable("x", Type<int>()),
                        Variable("x")),
                    Variable("a"));

            var actual = environment.Infer(expression);

            // (x:int -> x:int) a:int
            var expected =
                Apply(
                    Lambda(
                        BoundVariable("x", Type<int>()),
                        Variable("x", Type<int>())),
                    Variable("a", Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        
        [Test]
        public void ApplyverloadingExactMatch2()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Type<int>());
            environment.MutableBind("a", Type<double>());

            // (x:double -> x) a
            var expression =
                Apply(
                    Lambda(
                        BoundVariable("x", Type<double>()),
                        Variable("x")),
                    Variable("a"));

            var actual = environment.Infer(expression);

            // (x:double -> x:double) a:double
            var expected =
                Apply(
                    Lambda(
                        BoundVariable("x", Type<double>()),
                        Variable("x", Type<double>())),
                    Variable("a", Type<double>()));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
