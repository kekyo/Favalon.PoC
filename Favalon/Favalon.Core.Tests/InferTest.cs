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

        // TODO: Manually checked. Will fix to implement resolver for overload resolutions.
        //[Test]
        public void InferType()
        {
            // System.Int32
            var term = Term.Identity("System.Int32");

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            var expected = Term.Constant(typeof(int));

            Assert.AreEqual(expected, actual);
        }

        public sealed class TypeWithConstructor
        {
            public TypeWithConstructor(string arg)
            { }
        }

        [Test]
        public void InferValueConstructor()
        {
            // TypeWithConstructor(arg)
            var term = Term.Identity("Favalon.InferTest.TypeWithConstructor");

            var environment = Environment.Create();
            environment.AddBoundTermFromType(typeof(TypeWithConstructor));

            var actual = environment.Infer(term);

            var expected = Term.ValueConstructor(typeof(TypeWithConstructor));

            Assert.AreEqual(expected, actual);
        }

        public sealed class GenericTypeDefinition<T>
        {
        }

        [Test]
        public void InferTypeConstructor()
        {
            // GenericTypeDefinition<T>
            var term = Term.Identity("Favalon.InferTest.GenericTypeDefinition");

            var environment = Environment.Create();
            environment.AddBoundTermFromType(typeof(GenericTypeDefinition<>));

            var actual = environment.Infer(term);

            var expected = Term.TypeConstructor(typeof(GenericTypeDefinition<>));

            Assert.AreEqual(expected, actual);
        }
    }
}
