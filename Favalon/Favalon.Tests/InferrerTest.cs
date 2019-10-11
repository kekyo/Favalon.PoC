using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using static Favalon.Factories;

namespace Favalon
{
    [TestFixture]
    public sealed class InferrerTest
    {
        [Test]
        public void InferUnspecifiedVariable()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Variable("abc"));

            Assert.AreEqual(Variable("abc"), actual);
        }

        [Test]
        public void InferIntegerNumber()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Number("123"));

            var expected = Number(123);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InferDoubleNumber()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Number("123.456"));

            var expected = Number(123.456);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InferArrow()
        {
            // -> :: '0 -> '1 -> ('0 -> '1)
            // -> a b : a -> b

            var environment = Environment.Create();

            var actual = environment.Infer(Variable("->"));

            var expectedHigherOrder =
                Function(
                    Variable("'0"),
                    Function(
                        Variable("'1"),
                        Function(
                            Variable("'0"),
                            Variable("'1"))));
            var expected = Variable("->", expectedHigherOrder);
            Assert.AreEqual(expected, actual);
        }
    }
}
