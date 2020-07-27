using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;

using static Favalet.Generator;
using static Favalet.CLRGenerator;
using Favalet.Expressions.Specialized;

namespace Favalet
{
    [TestFixture]
    public sealed class HigherOrderTest
    {
        private static readonly TypeCalculator calculator =
            new TypeCalculator();

        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!calculator.ExactEquals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
            }
        }

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

        [Test]
        public void InferringWithAnnotation1()
        {
            var scope = Scope.Create();

            // true && false
            var expression =
                And(
                    Identity("true"),
                    Identity("false"));

            var actual = scope.Infer(expression);

            // true
            var ph0 = scope.UnsafeCreatePlaceholder(0);
            var expected =
                And(
                    Identity("true", ph0),
                    Identity("false", ph0),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
