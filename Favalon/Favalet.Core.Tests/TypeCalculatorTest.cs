using Favalet.Expressions;
using NUnit.Framework;
using System;
using System.Collections;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class TypeCalculatorTest
    {
        private static readonly TypeCalculator calculator =
            new TypeCalculator();

        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!calculator.Equals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringTypes.Simple),
                    expected.GetPrettyString(PrettyStringTypes.Simple),
                    actual.GetPrettyString(PrettyStringTypes.Simple));
            }
        }

        #region CombinedAnd
        [Test]
        public void NonReducibleCombinedAndTypes1()
        {
            var expression =
                AndBinary(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedAndTypes2()
        {
            var expression =
                AndBinary(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_1()
        {
            var expression =
                AndBinary(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_2()
        {
            var expression =
                AndBinary(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_1()
        {
            var expression =
                AndBinary(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<string>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_2()
        {
            var expression =
                AndBinary(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<string>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_1()
        {
            var expression =
                AndBinary(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_2()
        {
            var expression =
                AndBinary(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_1()
        {
            var expression =
                AndBinary(
                    Type<double>(),
                    AndBinary(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_2()
        {
            var expression =
                AndBinary(
                    Type<double>(),
                    AndBinary(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_3()
        {
            var expression =
                AndBinary(
                    Type<IFormattable>(),
                    AndBinary(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_1()
        {
            var expression =
                AndBinary(
                    Type<Queue>(),
                    AndBinary(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_2()
        {
            var expression =
                AndBinary(
                    Type<ICloneable>(),
                    AndBinary(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_3()
        {
            var expression =
                AndBinary(
                    Type<IEnumerable>(),
                    AndBinary(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region CombinedOr
        [Test]
        public void NonReducibleCombinedOrTypes1()
        {
            var expression =
                OrBinary(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedOrTypes2()
        {
            var expression =
                OrBinary(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_1()
        {
            var expression =
                OrBinary(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_2()
        {
            var expression =
                OrBinary(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_1()
        {
            var expression =
                OrBinary(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_2()
        {
            var expression =
                OrBinary(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_1()
        {
            var expression =
                OrBinary(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_2()
        {
            var expression =
                OrBinary(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_1()
        {
            var expression =
                OrBinary(
                    Type<double>(),
                    OrBinary(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_2()
        {
            var expression =
                OrBinary(
                    Type<double>(),
                    OrBinary(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_3()
        {
            var expression =
                OrBinary(
                    Type<IFormattable>(),
                    OrBinary(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_1()
        {
            var expression =
                OrBinary(
                    Type<Queue>(),
                    OrBinary(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_2()
        {
            var expression =
                OrBinary(
                    Type<ICloneable>(),
                    OrBinary(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_3()
        {
            var expression =
                OrBinary(
                    Type<IEnumerable>(),
                    OrBinary(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region CombinedBoth
        [Test]
        public void NonReducibleCombinedAndOrTypes()
        {
            var expression =
                AndBinary(
                    Type<double>(),
                    OrBinary(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            var expected =
                AndBinary(
                    Type<double>(),
                    OrBinary(
                        Type<int>(),
                        Type<string>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedOrAndTypes()
        {
            var expression =
                OrBinary(
                    Type<double>(),
                    AndBinary(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            var expected =
                OrBinary(
                    Type<double>(),
                    AndBinary(
                        Type<int>(),
                        Type<string>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndOrTypes()
        {
            // IFormattable && (Int32 || String)
            var expression =
                AndBinary(
                    Type<IFormattable>(),
                    OrBinary(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            // Int32
            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrAndTypes()
        {
            // IFormattable || (Int32 && String)
            var expression =
                OrBinary(
                    Type<IFormattable>(),
                    AndBinary(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion
    }
}
