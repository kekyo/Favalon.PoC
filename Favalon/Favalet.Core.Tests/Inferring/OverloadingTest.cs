using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class SubTypingTest
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
        
        #region Widening
        [Test]
        public void Widening1()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Type<int>());

            // a:object
            var expression =
                Variable("a", Type<object>());

            var actual = environment.Infer(expression);

            // a:object
            var expected =
                Variable("a", Type<object>());

            AssertLogicalEqual(expression, expected, actual);
        }
        
        //[Test]
        public void Widening2()
        {
            var environment = CLREnvironment();
            
            environment.MutableBind("a", Type<object>());

            // a:int
            var expression =
                Variable("a", Type<int>());

            var actual = environment.Infer(expression);

            // TODO: really? cause error?
            // a:object
            var expected =
                Variable("a", Type<object>());

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region Covariance
        [Test]
        public void CovarianceInLambdaBody1()
        {
            var environment = CLREnvironment();

            // (a -> a):(int -> object)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a"),
                    Function(
                        Type<int>(),
                        Type<object>()));

            var actual = environment.Infer(expression);

            // (a:int -> a:int):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<int>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void CovarianceInLambdaBody2()
        {
            var environment = CLREnvironment();

            // (a:int -> a):(_ -> object)
            var expression =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a"),
                    Function(
                        Unspecified(),
                        Type<object>()));

            var actual = environment.Infer(expression);

            // (a:int -> a:int):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<int>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void CovarianceInLambdaBody3()
        {
            var environment = CLREnvironment();

            // (a -> a:int):(_ -> object)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a", Type<int>()),
                    Function(
                        Unspecified(),
                        Type<object>()));

            var actual = environment.Infer(expression);

            // (a:int -> a:int):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<int>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region Contravariance
        [Test]
        public void ContravarianceInLambdaBody1()
        {
            var environment = CLREnvironment();

            // (a:object -> a):(int -> object)
            var expression =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a"),
                    Function(
                        Type<int>(),
                        Type<object>()));

            var actual = environment.Infer(expression);

            // (a:object -> a:object):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        
        [Test]
        public void ContravarianceInLambdaBody2()
        {
            var environment = CLREnvironment();

            // (a -> a:object):(int -> object)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            var actual = environment.Infer(expression);

            // (a:object -> a:object):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        
        [Test]
        public void ContravarianceInLambdaBody3()
        {
            var environment = CLREnvironment();

            // (a:object -> a):(int -> _)
            var expression =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a"),
                    Function(
                        Type<int>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:object -> a:object):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        
        [Test]
        public void ContravarianceInLambdaBody4()
        {
            var environment = CLREnvironment();

            // (a -> a:object):(int -> _)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:object -> a:object):(int -> object)
            var expected =
                Lambda(
                    BoundVariable("a", Type<object>()),
                    Variable("a", Type<object>()),
                    Function(
                        Type<int>(),
                        Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion
    }
}
