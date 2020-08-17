using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;
using Favalet.Expressions.Specialized;
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
        public void TypeVariable1()
        {
            var environment = CLREnvironment();

            // a:bool -> a
            var expression =
                Lambda(
                    BoundVariable("a", Variable("bool")),
                    Variable("a"));

            var actual = environment.Infer(expression);

            // a:bool -> a:bool
            var expected =
                Lambda(
                    BoundVariable("a", Variable("bool")),
                    Variable("a", Variable("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }
 
        [Test]
        public void TypeVariable2()
        {
            var environment = CLREnvironment();

            // a -> a:bool
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a", Variable("bool")));

            var actual = environment.Infer(expression);

            // a:bool -> a:bool
            var expected =
                Lambda(
                    BoundVariable("a", Variable("bool")),
                    Variable("a", Variable("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }
 
        [Test]
        public void TypeVariable3()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a"),
                    Function(
                        Variable("bool"),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundVariable("a", Variable("bool")),
                    Variable("a", Variable("bool")),
                    Function(
                        Variable("bool"),
                        Variable("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }
 
        [Test]
        public void TypeVariable4()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool)
            var expression =
                Lambda(
                    BoundVariable("a"),
                    Variable("a"),
                    Function(
                        Unspecified(),
                        Variable("bool")));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundVariable("a", Variable("bool")),
                    Variable("a", Variable("bool")),
                    Function(
                        Variable("bool"),
                        Variable("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void Placeholder1()
        {
            var environment = CLREnvironment();

            // a:int -> a:object
            var expression =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<object>()));

            var actual = environment.Infer(expression);

            // a:int -> a:object
            var expected =
                Lambda(
                    BoundVariable("a", Type<int>()),
                    Variable("a", Type<object>()));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
