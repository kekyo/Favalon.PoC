using Favalet.Expressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using static Favalet.Generator;
using static Favalet.CLRGenerator;

namespace Favalet
{
    [TestFixture]
    public sealed class TypeCalculatorTest
    {
        [Test]
        public void NonReduceSingleAnd()
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
    }
}
