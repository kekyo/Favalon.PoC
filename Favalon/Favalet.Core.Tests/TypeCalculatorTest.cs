using Favalet.Contexts;
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
            if (!calculator.ExactEquals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
            }
        }

        #region CombinedAnd
        [Test]
        public void NonReducibleCombinedAndTypes1()
        {
            // int && string
            var expression =
                And(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            // int && string
            var expected =
                And(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedAndTypes2()
        {
            // string && int
            var expression =
                And(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // int && string
            var expected =
                And(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_1()
        {
            // object && int
            var expression =
                And(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // int
            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_2()
        {
            // int && object
            var expression =
                And(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            // int
            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_1()
        {
            // object && string
            var expression =
                And(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            // string
            var expected =
                Type<string>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_2()
        {
            // string && object
            var expression =
                And(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            // string
            var expected =
                Type<string>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_1()
        {
            // IFormattable && int
            var expression =
                And(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // int
            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_2()
        {
            // int && IFormattable
            var expression =
                And(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            // int
            var expected =
                Type<int>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_1()
        {
            // double && IFormattable && int
            var expression =
                And(
                    Type<double>(),
                    And(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            // double && int
            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_2()
        {
            // double && int && IFormattable
            var expression =
                And(
                    Type<double>(),
                    And(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            // double && int
            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_3()
        {
            // IFormattable && int && double
            var expression =
                And(
                    Type<IFormattable>(),
                    And(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            // double && int
            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_1()
        {
            // Queue && ICloneable && IEnumerable
            var expression =
                And(
                    Type<Queue>(),
                    And(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            // Queue
            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_2()
        {
            // ICloneable && Queue && IEnumerable
            var expression =
                And(
                    Type<ICloneable>(),
                    And(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            // Queue
            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_3()
        {
            // IEnumerable && ICloneable && Queue
            var expression =
                And(
                    Type<IEnumerable>(),
                    And(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            // Queue
            var expected =
                Type<Queue>();

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region CombinedOr
        [Test]
        public void NonReducibleCombinedOrTypes1()
        {
            // int || string
            var expression =
                Or(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            // int || string
            var expected =
                Or(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedOrTypes2()
        {
            // string || int
            var expression =
                Or(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // int || string
            var expected =
                Or(
                    Type<int>(),
                    Type<string>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_1()
        {
            // object || int
            var expression =
                Or(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // object
            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_2()
        {
            // int || object
            var expression =
                Or(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            // object
            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_1()
        {
            // object || string
            var expression =
                Or(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            // object
            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_2()
        {
            // string || object
            var expression =
                Or(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            // object
            var expected =
                Type<object>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_1()
        {
            // IFormattable || int
            var expression =
                Or(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_2()
        {
            // int || IFormattable
            var expression =
                Or(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_1()
        {
            // double || IFormattable || int
            var expression =
                Or(
                    Type<double>(),
                    Or(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_2()
        {
            // double || int || IFormattable
            var expression =
                Or(
                    Type<double>(),
                    Or(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_3()
        {
            // IFormattable || int || double
            var expression =
                Or(
                    Type<IFormattable>(),
                    Or(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            // IFormattable
            var expected =
                Type<IFormattable>();

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_1()
        {
            // Queue || ICloneable || IEnumerable
            var expression =
                Or(
                    Type<Queue>(),
                    Or(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            // ICloneable || IEnumerable
            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_2()
        {
            // ICloneable || Queue || IEnumerable
            var expression =
                Or(
                    Type<ICloneable>(),
                    Or(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            // ICloneable || IEnumerable
            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_3()
        {
            // IEnumerable || ICloneable || Queue
            var expression =
                Or(
                    Type<IEnumerable>(),
                    Or(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            // ICloneable || IEnumerable
            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region CombinedBoth
        [Test]
        public void NonReducibleCombinedAndOrTypes()
        {
            // double && (int || string)
            var expression =
                And(
                    Type<double>(),
                    Or(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            // double && (int || string)
            var expected =
                And(
                    Type<double>(),
                    Or(
                        Type<int>(),
                        Type<string>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReducibleCombinedOrAndTypes()
        {
            // double || (int && string)
            var expression =
                Or(
                    Type<double>(),
                    And(
                        Type<int>(),
                        Type<string>()));

            var actual = calculator.Compute(expression);

            // double || (int && string)
            var expected =
                Or(
                    Type<double>(),
                    And(
                        Type<int>(),
                        Type<string>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndOrTypes()
        {
            // IFormattable && (Int32 || String)
            var expression =
                And(
                    Type<IFormattable>(),
                    Or(
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
                Or(
                    Type<IFormattable>(),
                    And(
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
