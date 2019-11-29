using Favalon.Terms;
using NUnit.Framework;
using System;

namespace Favalon
{
    [TestFixture]
    public sealed class InferTest
    {
        [Test]
        public void InferInt32Constant()
        {
            // 123
            var term = Term.Constant(123);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual("System.Int32", actual.HigherOrder.Readable);
        }

        [Test]
        public void InferStaticMethod()
        {
            // System.Int32.Parse: System.String -> System.Int32
            var term = Term.Identity("System.Int32.Parse");

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual("System.Int32", actual.HigherOrder.Readable);
        }

        [Test]
        public void InferType()
        {
            // System.Int32
            var term = Term.Identity("System.Int32");

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            var expected = Term.Constant(typeof(int));

            Assert.AreEqual(expected, actual);
        }

        public sealed class GenericTypeNotConstructor<T>
        {
        }

        [Test]
        public void InferGenericType()
        {
            // System.Lazy<T>
            var term = Term.Identity("Favalon.InferTest.GenericTypeNotConstructor");

            var environment = Environment.Create();
            environment.AddBoundTermFromType(typeof(GenericTypeNotConstructor<>));

            var actual = environment.Infer(term);

            var expected = Term.Constant(typeof(GenericTypeNotConstructor<>));

            Assert.AreEqual(expected, actual);
        }
    }
}
