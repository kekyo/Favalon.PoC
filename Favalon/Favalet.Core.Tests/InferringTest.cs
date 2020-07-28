using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class InferringTest
    {
        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!expected.Equals(actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
            }
        }

        #region UnspecifiedType
        [Test]
        public void UnspecifiedTypeInferring1()
        {
            var scope = Scope.Create();

            scope.SetVariable(
                "true",
                Constant(true));

            // true && false
            var expression =
                Identity("true");

            var actual = scope.Infer(expression);

            // true:bool
            var expected =
                Identity("true", Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void UnspecifiedTypeInferring2()
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

        #region And
        [Test]
        public void InferringAndWithoutAnnotation1()
        {
            var scope = Scope.Create();

            // true && false
            var expression =
                And(
                    Identity("true"),
                    Identity("false"));

            var actual = scope.Infer(expression);

            // (true:'0 && false:'0):'0
            var ph0 = PseudoPlaceholderTerm.Create(0);
            var expected =
                And(
                    Identity("true", ph0),
                    Identity("false", ph0),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithoutAnnotation2()
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
            var ph0 = PseudoPlaceholderTerm.Create(0);
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

        [Test]
        public void InferringAndWithAnnotation11()
        {
            var scope = Scope.Create();

            // (true && false):bool
            var expression =
                And(
                    Identity("true"),
                    Identity("false"),
                    Type<bool>());

            var actual = scope.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation12()
        {
            var scope = Scope.Create();

            // true:bool && false
            var expression =
                And(
                    Identity("true", Type<bool>()),
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

        [Test]
        public void InferringAndWithAnnotation13()
        {
            var scope = Scope.Create();

            // true && false:bool
            var expression =
                And(
                    Identity("true"),
                    Identity("false", Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation14()
        {
            var scope = Scope.Create();

            // true:bool && false:bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation15()
        {
            var scope = Scope.Create();

            // (true:bool && false:bool):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            var actual = scope.Infer(expression);

            // (true:bool && false:bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    Identity("false", Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        /////////////////////////////////////////////////////

        [Test]
        public void InferringAndWithAnnotation21()
        {
            var scope = Scope.Create();

            // (true && (false && true)):bool
            var expression =
                And(
                    Identity("true"),
                    And(
                        Identity("false"),
                        Identity("true")),
                    Type<bool>());

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation22()
        {
            var scope = Scope.Create();

            // true:bool && (false && true)
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false"),
                        Identity("true")));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation23()
        {
            var scope = Scope.Create();

            // true && (false:bool && true)
            var expression =
                And(
                    Identity("true"),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true")));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation24()
        {
            var scope = Scope.Create();

            // true && (false && true:bool)
            var expression =
                And(
                    Identity("true"),
                    And(
                        Identity("false"),
                        Identity("true", Type<bool>())));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation25()
        {
            var scope = Scope.Create();

            // true && (false && true):bool
            var expression =
                And(
                    Identity("true"),
                    And(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation26()
        {
            var scope = Scope.Create();

            // true:bool && (false:bool && true)
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true")));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation27()
        {
            var scope = Scope.Create();

            // true && (false:bool && true:bool)
            var expression =
                And(
                    Identity("true"),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>())));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation28()
        {
            var scope = Scope.Create();

            // true:bool && (false && true:bool)
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false"),
                        Identity("true", Type<bool>())));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation29()
        {
            var scope = Scope.Create();

            // true:bool && (false:bool && true:bool)
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>())));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation30()
        {
            var scope = Scope.Create();

            // true:bool && (false && true):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false"),
                        Identity("true"),
                        Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation31()
        {
            var scope = Scope.Create();

            // true:bool && (false:bool && true):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true"),
                        Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation32()
        {
            var scope = Scope.Create();

            // true:bool && (false && true:bool):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false"),
                        Identity("true", Type<bool>()),
                        Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation33()
        {
            var scope = Scope.Create();

            // true:bool && (false:bool && true:bool):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()));

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void InferringAndWithAnnotation34()
        {
            var scope = Scope.Create();

            // (true:bool && (false:bool && true:bool):bool):bool
            var expression =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            var actual = scope.Infer(expression);

            // (true:bool && (false:bool && true:bool):bool):bool
            var expected =
                And(
                    Identity("true", Type<bool>()),
                    And(
                        Identity("false", Type<bool>()),
                        Identity("true", Type<bool>()),
                        Type<bool>()),
                    Type<bool>());

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion


        // TODO: Orについてテストする
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
