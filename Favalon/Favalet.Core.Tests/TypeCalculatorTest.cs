﻿using Favalet.Expressions;
using NUnit.Framework;
using System;
using System.Collections;

using static Favalet.Generator;
using static Favalet.CLRGenerator;

namespace Favalet
{
    [TestFixture]
    public sealed class TypeCalculatorTest
    {
        #region CombinedOr
        [Test]
        public void NonReducibleCombinedAndTypes1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                And(
                    Type<int>(),
                    Type<string>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NonReducibleCombinedAndTypes2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                And(
                    Type<int>(),
                    Type<string>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes1_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<string>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes2_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<string>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedAndTypes3_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<int>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<double>(),
                    AndBinary(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<double>(),
                    AndBinary(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes1_3()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<IFormattable>(),
                    AndBinary(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            var expected =
                And(
                    Type<double>(),
                    Type<int>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<Queue>(),
                    AndBinary(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<ICloneable>(),
                    AndBinary(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedAndTypes2_3()
        {
            var calculator = new TypeCalculator();

            var expression =
                AndBinary(
                    Type<IEnumerable>(),
                    AndBinary(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<Queue>();

            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region CombinedOr
        [Test]
        public void NonReducibleCombinedOrTypes1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<int>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                Or(
                    Type<int>(),
                    Type<string>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NonReducibleCombinedOrTypes2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<string>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Or(
                    Type<string>(),
                    Type<int>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<object>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes1_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<int>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<object>(),
                    Type<string>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes2_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<string>(),
                    Type<object>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<object>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<IFormattable>(),
                    Type<int>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleCombinedOrTypes3_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<int>(),
                    Type<IFormattable>());

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<double>(),
                    OrBinary(
                        Type<IFormattable>(),
                        Type<int>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<double>(),
                    OrBinary(
                        Type<int>(),
                        Type<IFormattable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes1_3()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<IFormattable>(),
                    OrBinary(
                        Type<int>(),
                        Type<double>()));

            var actual = calculator.Compute(expression);

            var expected =
                Type<IFormattable>();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_1()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<Queue>(),
                    OrBinary(
                        Type<ICloneable>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_2()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<ICloneable>(),
                    OrBinary(
                        Type<Queue>(),
                        Type<IEnumerable>()));

            var actual = calculator.Compute(expression);

            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ReducibleMultipleCombinedOrTypes2_3()
        {
            var calculator = new TypeCalculator();

            var expression =
                OrBinary(
                    Type<IEnumerable>(),
                    OrBinary(
                        Type<ICloneable>(),
                        Type<Queue>()));

            var actual = calculator.Compute(expression);

            var expected =
                Or(
                    Type<ICloneable>(),
                    Type<IEnumerable>());

            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
