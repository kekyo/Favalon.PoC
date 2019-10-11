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
        public void InferNotBoundVariable()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Variable("abc"));

            Assert.AreEqual(Variable("abc"), actual);
        }

        [Test]
        public void InferBoundVariable()
        {
            var environment = Environment.Create();
            var environment2 = environment.Bind("abc", Number(123));

            var actual = environment2.Infer(Variable("abc"));

            Assert.AreEqual(Number(123), actual);
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
        public void InferDefaultBoundArrow()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Variable("->"));

            var expected = environment.BoundTerms["->"];
            Assert.AreEqual(expected, actual);
        }
    }
}
