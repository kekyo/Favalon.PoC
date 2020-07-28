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
            if (!expected.ExactEquals(actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
            }
        }

        private static void AssertLogicalEqual(
            PseudoPlaceholderProvider provider,
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!provider.Equals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
            }
        }
        #endregion

        #region From variable
        [Test]
        public void InferringFromVariable1()
        {
            var scope = Scope.Create();

            scope.SetVariable(
                "true",
                Constant(true));

            // true
            var expression =
                Identity("true");

            var actual = scope.Infer(expression);

            // true:bool
            var expected =
                Identity("true", Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringFromVariable2()
        {
            var scope = Scope.Create();

            scope.SetVariable(
                "true",
                Constant(true));
            scope.SetVariable(
                "false",
                Constant(false));

            // true && false
            var expression =
                And(
                    Identity("true"),
                    Identity("false"));

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && false
            var expression =
                oper(
                    Identity("true"),
                    Identity("false"),
                    null);

            var actual = scope.Infer(expression);

            // (true:'0 && false:'0):'0
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                oper(
                    Identity("true", ph0),
                    Identity("false", ph0),
                    ph0);

            AssertLogicalEqual(provider, expression, expected, actual);
        }

        [TestCaseSource("BinaryOperators")]
        public void InferringBinaryWithoutAnnotation2(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var scope = Scope.Create();

            // (true && false) && true
            var expression =
                And(
                    And(
                        Identity("true"),
                        Identity("false")),
                    Identity("true"));

            var actual = scope.Infer(expression);

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

            AssertLogicalEqual(provider, expression, expected, actual);
        }

        /////////////////////////////////////////////////////

        [TestCaseSource("BinaryOperators")]
        public void InferringBinaryWithAnnotation11(
            Func<IExpression, IExpression, IExpression?, IExpression> oper)
        {
            var scope = Scope.Create();

            // (true && false):bool
            var expression =
                oper(
                    Identity("true"),
                    Identity("false"),
                    Type<bool>());

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && false
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false"),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && false:bool
            var expression =
                oper(
                    Identity("true"),
                    Identity("false", Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && false:bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // (true:bool && false:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // (true && (false && true)):bool
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        null),
                    Type<bool>());

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false && true)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && (false:bool && true)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && (false && true:bool)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && (false && true):bool
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false:bool && true)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true && (false:bool && true:bool)
            var expression =
                oper(
                    Identity("true"),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false && true:bool)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false:bool && true:bool)
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        null),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false && true):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false:bool && true):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false && true:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // true:bool && (false:bool && true:bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    null);

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // (true:bool && (false:bool && true:bool):bool):bool
            var expression =
                oper(
                    Identity("true", Type<bool>()),
                    oper(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            var actual = scope.Infer(expression);

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
            var scope = Scope.Create();

            // a -> a
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"));

            var actual = scope.Infer(expression);

            // (a:'0 -> a:'0):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("a", ph0),
                    Function(ph0, ph0));

            AssertLogicalEqual(provider, expression, expected, actual);
        }
        #endregion



        // TODO: Lambdaについてテストする
        // TODO: Functionについてテストする
        // TODO: Applyについてテストする


        // TODO: 異なる型でInferする場合をテストする
        //    1: 代入互換性が無いものはその場でエラー？ 又はASTを変形しないで返すとか？
        //    2: 代入互換性があるもの
        //    2-1: 正方向？（例えば引数）
        //    2-2: 逆方向？（例えば戻り値）


        // TODO: Unmatched orders
    }
}
