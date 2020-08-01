using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class InferringTest
    {
        #region AssertLogicalEqual
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
        #endregion

        #region From variable
        [Test]
        public void InferringFromVariable1()
        {
            var environment = CLREnvironment();

            environment.MutableBind(
                "true",
                Constant(true));

            // true
            var expression =
                Identity("true");

            var actual = environment.Infer(expression);

            // true:bool
            var expected =
                Identity("true", Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringFromVariable2()
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
                    Identity("true"),
                    Identity("false"));

            var actual = environment.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region Binary operators
        private static readonly Func<IExpression, IExpression, IExpression?, IExpression>[] BinaryOperators =
            new[]
            {
                new Func<IExpression, IExpression, IExpression?, IExpression>((lhs, rhs, ho) =>
                    ho is IExpression ? And(lhs, rhs, ho) : And(lhs, rhs)),
                new Func<IExpression, IExpression, IExpression?, IExpression>((lhs, rhs, ho) =>
                    ho is IExpression ? Or(lhs, rhs, ho) : Or(lhs, rhs)),
            };

        [TestCaseSource("BinaryOperators")]
        public void InferringBinaryWithoutAnnotation1(
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
        public void InferringBinaryWithoutAnnotation2(
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
        public void InferringBinaryWithAnnotation11(
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
        public void InferringBinaryWithAnnotation12(
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
        public void InferringBinaryWithAnnotation13(
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
        public void InferringBinaryWithAnnotation14(
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
        public void InferringBinaryWithAnnotation15(
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
        public void InferringBinaryWithAnnotation21(
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
        public void InferringBinaryWithAnnotation22(
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
        public void InferringBinaryWithAnnotation23(
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
        public void InferringBinaryWithAnnotation24(
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
        public void InferringBinaryWithAnnotation25(
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
        public void InferringBinaryWithAnnotation26(
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
        public void InferringBinaryWithAnnotation27(
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
        public void InferringBinaryWithAnnotation28(
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
        public void InferringBinaryWithAnnotation29(
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
        public void InferringBinaryWithAnnotation30(
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
        public void InferringBinaryWithAnnotation31(
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
        public void InferringBinaryWithAnnotation32(
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
        public void InferringBinaryWithAnnotation33(
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
        public void InferringBinaryWithAnnotation34(
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
        #endregion

        #region Lambda
        [Test]
        public void InferringLambdaWithoutAnnotation1()
        {
            var environment = CLREnvironment();

            // a -> a
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:'0 -> a:'0):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("a", ph0),
                    Function(ph0, ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithoutAnnotation2()
        {
            var environment = CLREnvironment();

            // a -> b
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:'1):('0 -> '1)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", ph1),
                    Function(ph0, ph1));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a:bool -> a
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a", Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:bool -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation4()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation5()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation6()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Type<bool>(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation7()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:'0 -> a:'0):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("a", ph0),
                    Function(ph0, ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation8()
        {
            var environment = CLREnvironment();

            // (a -> b):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:'0):(bool -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", ph0),
                    Function(Type<bool>(), ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation9()
        {
            var environment = CLREnvironment();

            // (a -> b):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:bool):('0 -> bool)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", Type<bool>()),
                    Function(ph0, Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation10()
        {
            var environment = CLREnvironment();

            // (a -> b):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Type<bool>(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation11()
        {
            var environment = CLREnvironment();

            // (a -> b:bool):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b", Type<bool>()),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation12()
        {
            var environment = CLREnvironment();

            // (a:bool -> b):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation13_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation13_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation14_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            var r = expected.GetPrettyString(PrettyStringTypes.Readable);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation14_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            var r = expected.GetPrettyString(PrettyStringTypes.Readable);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation15_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation15_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation16()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(* -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Kind(),
                            Unspecified())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation17()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(_ -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Unspecified(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation18()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(* -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Unspecified())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaWithAnnotation19()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(_ -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Unspecified(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool:* -> a:bool:*):(bool:* -> bool:*):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaShadowedVariable1()
        {
            var environment = CLREnvironment();

            // a = c:int
            environment.MutableBind(
                "a",
                Identity("c", Type<int>()));

            // a -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaShadowedVariable2()
        {
            var environment = CLREnvironment();

            // b:int = c
            environment.MutableBind(
                BoundSymbol("b", Type<int>()),
                Identity("c"));

            // a:bool -> b
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:bool -> b:int):(bool -> int)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<int>()),
                    Function(Type<bool>(), Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaShadowedVariable3()
        {
            var environment = CLREnvironment();

            // b = 123:int
            environment.MutableBind(
                "b",
                Constant(123));

            // a:bool -> b
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:bool -> b:int):(bool -> int)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<int>()),
                    Function(Type<bool>(), Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringLambdaComplex1()
        {
            var environment = CLREnvironment();

            // b = 123:int
            environment.MutableBind(
                "b",
                Constant(123));

            // a -> b
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:int):('0 -> int)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", Type<int>()),
                    Function(ph0, Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region Apply
        [Test]
        public void InferringApplyWithoutAnnotation()
        {
            var environment = CLREnvironment();

            // a b
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:('0 -> '1) b:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a", Function(ph0, ph1)),
                    Identity("b", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyNestedFunctionWithoutAnnotation1()
        {
            var environment = CLREnvironment();

            // a a
            var expression =
                Apply(
                    Identity("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:('0 -> '1) a:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a", Function(ph0, ph1)),
                    Identity("a", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyNestedFunctionWithoutAnnotation2()
        {
            var environment = CLREnvironment();

            // a = x -> x
            environment.MutableBind(
                "a",
                Lambda(
                    "x",
                    Identity("x")));

            // a a
            var expression =
                Apply(
                    Identity("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // ((x:('0 -> '0) -> x:('0 -> '0)) (x:'0 -> x:'0)):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity(
                        "a",
                        Function(
                            Function(ph0, ph0),
                            Function(ph0, ph0))),
                    Identity(
                        "a",
                        Function(ph0, ph0)),
                    Function(
                        ph0,
                        ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a:(bool -> _) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:(bool -> '0) b:bool):'0
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(Identity("bool"), ph0)),
                    Identity("b", Identity("bool")),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a:(_ -> bool) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("bool"))),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:('0 -> bool) b:'0):bool
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(ph0, Identity("bool"))),
                    Identity("b", ph0),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:(int -> bool) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("int"),
                            Identity("bool"))),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:(int -> bool) b:int):bool
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("int"),
                            Identity("bool"))),
                    Identity("b", Identity("int")),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation4()
        {
            var environment = CLREnvironment();

            // a b:bool
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> _) b:bool):_
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation5()
        {
            var environment = CLREnvironment();

            // (a b):bool
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b"),
                    Identity("bool"));

            var actual = environment.Infer(expression);

            // (a:('0 -> bool) b:'0):bool
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            ph0,
                            Identity("bool"))),
                    Identity("b", ph0),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation6()
        {
            var environment = CLREnvironment();

            // (a b:bool):int
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation7()
        {
            var environment = CLREnvironment();

            // a:(_ -> int) b:bool
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("int"))),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation8()
        {
            var environment = CLREnvironment();

            // a:(_ -> int) b:bool
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("int"))),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation9()
        {
            var environment = CLREnvironment();

            // (a:(bool -> _) b):int
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("int"))),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringApplyWithAnnotation10()
        {
            var environment = CLREnvironment();

            // (a:(bool -> int) b:bool):int
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringNestedApplyWithoutAnnotation()
        {
            var environment = CLREnvironment();

            // a b c
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b")),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:('0 -> ('1 -> '2)) b:'0 c:'1):'2
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var ph2 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                ph0,
                                Function(ph1, ph2))),
                        Identity("b", ph0)),
                    Identity("c", ph1),
                    ph2);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringNestedApplyWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a b c:bool
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b")),
                    Identity("c", Identity("bool")));

            var actual = environment.Infer(expression);

            // ((a:('0 -> (bool -> '1)) b:'0 c:bool):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                ph0,
                                Function(Identity("bool"), ph1))),
                        Identity("b", ph0)),
                    Identity("c", Identity("bool")),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringNestedApplyWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a b:bool c
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b", Identity("bool"))),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:(bool -> ('0 -> '1)) b:bool c:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                Identity("bool"),
                                Function(ph0, ph1))),
                        Identity("b", Identity("bool"))),
                    Identity("c", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringNestedApplyWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:(bool -> _) b c
            var expression =
                Apply(
                    Apply(
                        Identity("a",
                            Function(Identity("bool"), Unspecified())),
                        Identity("b")),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:(bool -> ('0 -> '1)) b:bool c:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                Identity("bool"),
                                Function(ph0, ph1))),
                        Identity("b", Identity("bool"))),
                    Identity("c", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void InferringApplyYCombinator1()
        {
            var environment = CLREnvironment();

            // Y = f -> f (Y f)
            environment.MutableBind(
                "Y",
                Lambda(
                    "f",
                    Apply(
                        Identity("f"),
                        Apply(
                            Identity("Y"),
                            Identity("f")))));

            // Y
            var expression =
                Identity("Y");

            var actual = environment.Infer(expression);

            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void InferringApplyYCombinator2()
        {
            var environment = CLREnvironment();

            // Y = f -> (x -> f (x x)) (x -> f (x x))
            var expression =
                Lambda(
                    "f",
                    Apply(
                        Lambda( // x -> f (x x)
                            "x",
                            Apply(
                                Identity("f"),
                                Apply(
                                    Identity("x"),
                                    Identity("x")))),
                        Lambda( // x -> f (x x)
                            "x",
                            Apply(
                                Identity("f"),
                                Apply(
                                    Identity("x"),
                                    Identity("x"))))));

            var actual = environment.Infer(expression);

            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void InferringApplyZCombinator()
        {
            var environment = CLREnvironment();

            // Z = f -> (x -> f (y -> x x y)) (x -> f (y -> x x y))
            var expression =
                Lambda(
                    "f",
                    Apply(
                        Lambda( // x -> f (y -> x x y)
                            "x",
                            Apply(
                                Identity("f"),
                                Lambda( // y -> x x y
                                    "y",
                                    Apply(
                                        Apply(
                                            Identity("x"),
                                            Identity("x")),
                                        Identity("y"))))),
                        Lambda( // x -> f (y -> x x y)
                            "x",
                            Apply(
                                Identity("f"),
                                Lambda( // y -> x x y
                                    "y",
                                    Apply(
                                        Apply(
                                            Identity("x"),
                                            Identity("x")),
                                        Identity("y")))))));

            var actual = environment.Infer(expression);

            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion



        // TODO: 異なる型でInferする場合をテストする
        //    1: 代入互換性が無いものはその場でエラー？ 又はASTを変形しないで返すとか？
        //    2: 代入互換性があるもの
        //    2-1: 正方向？（例えば引数）
        //    2-2: 逆方向？（例えば戻り値）


        // TODO: Unmatched orders
    }
}
