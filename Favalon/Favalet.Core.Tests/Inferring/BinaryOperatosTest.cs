using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class BinaryOperatosTest
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

        private static readonly Func<IExpression, IExpression, IExpression?, IExpression>[] BinaryOperators =
            new[]
            {
                new Func<IExpression, IExpression, IExpression?, IExpression>((lhs, rhs, ho) =>
                    ho is IExpression ? And(lhs, rhs, ho) : And(lhs, rhs)),
                new Func<IExpression, IExpression, IExpression?, IExpression>((lhs, rhs, ho) =>
                    ho is IExpression ? Or(lhs, rhs, ho) : Or(lhs, rhs)),
            };

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithoutAnnotation1(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && false
            var expression =
                oper(
                    Identity("true"),
                    Identity("false"),
                    null);

            var actual = environment.Infer(expression);

            // (true:'0 && false:'0):'0
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                oper(
                    Identity("true", ph0),
                    Identity("false", ph0),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithoutAnnotation2(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // (true && false) && true
            var expression =
                And(
                    And(
                        Identity("true"),
                        Identity("false")),
                    Identity("true"));

            var actual = environment.Infer(expression);

            // (true:'0 && false:'0) && true:'0
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                And(
                    And(
                        Identity("true", ph0),
                        Identity("false", ph0),
                        ph0),
                    Identity("true", ph0),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }

        /////////////////////////////////////////////////////

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation11(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // (true && false):bool
            var expression =
                oper(
                    Identity("true"),
                    Identity("false"),
                    Type<bool>());

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation12(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && false
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false"),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation13(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && false:bool
            var expression =
                oper(
                    Identity("true"),
                    Identity("false", Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation14(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && false:bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation15(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // (true:bool && false:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        /////////////////////////////////////////////////////

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation21(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // (true && (false && true)):bool
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        null),
                    Type<bool>());

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation22(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false && true)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation23(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && (false:bool && true)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation24(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && (false && true:bool)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation25(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && (false && true):bool
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation26(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false:bool && true)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation27(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true && (false:bool && true:bool)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation28(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false && true:bool)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation29(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false:bool && true:bool)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation30(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false && true):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation31(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false:bool && true):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation32(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false && true:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation33(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // true:bool && (false:bool && true:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    null);

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void BinaryWithAnnotation34(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var environment = CLREnvironment();

            // (true:bool && (false:bool && true:bool):bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            var actual = environment.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
