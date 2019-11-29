using Favalon.Terms;
using NUnit.Framework;
using System;
using System.Linq;

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
            var actual = environment.Infer(term).Single();

            Assert.AreEqual("System.Int32", actual.HigherOrder!.Readable);
        }

        [Test]
        public void InferStaticMethod()
        {
            // System.Int32.Parse: System.String -> System.Int32
            var term = Term.Identity("System.Int32.Parse");

            var environment = Environment.Create();
            var actual = environment.Infer(term).Single();

            Assert.AreEqual("System.Int32.Parse(s:System.String) -> System.Int32", actual.Readable);
        }

        [Test]
        public void InferStaticMethod2()
        {
            // System.Math.Abs
            var term =
                Term.Apply(
                    Term.Identity("System.Math.Abs"),
                    Term.Constant(123));

            var environment = Environment.Create();
            var actual = environment.Infer(term).Single();

            Assert.AreEqual("System.Math.Abs 123", actual.Readable);
        }

        [Test]
        public void InferType()
        {
            // System.Int32
            var term = Term.Identity("System.Int32");

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            var expected = new[]
            {
                Term.Constant(typeof(int))
            };

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

            var expected = new Term[]
            {
                Term.Type(typeof(TypeWithConstructor)),
                Term.ValueConstructor(typeof(TypeWithConstructor))
            };

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

            var expected = new[] {
                Term.Type(typeof(GenericTypeDefinition<>)),
                Term.TypeConstructor(typeof(GenericTypeDefinition<>))
            };

            Assert.AreEqual(expected, actual);
        }
    }
}
