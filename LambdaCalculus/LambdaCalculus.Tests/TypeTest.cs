using Favalon.Types;
using NUnit.Framework;
using System;

namespace Favalon
{
    [TestFixture]
    class TypeTest
    {
        [TestCase(typeof(int), 123)]
        [TestCase(typeof(string), "abc")]
        [TestCase(typeof(bool), false)]
        public void Order1(Type type, object value)
        {
            var term =
                Term.Constant(value);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Type(type), actual.HigherOrder);
        }

        [Test]
        public void ComposeTypeConstructor()
        {
            var term =
                Term.Apply(
                    Term.Type(typeof(Lazy<>)),
                    Term.Type<int>());

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Type<Lazy<int>>(), actual);
        }

        public sealed class ComposeConstructorTarget
        {
            public readonly int Value;

            public ComposeConstructorTarget(int value) =>
                this.Value = value;
        }

        public void ComposeConstructor()
        {
            var term =
                Term.Apply(
                    Term.Type<ComposeConstructorTarget>(),
                    Term.Constant(123));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(123, ((ComposeConstructorTarget)((ConstantTerm)actual).Value).Value);
        }

        //[Test]
        public void ComposeDiscriminatedUnionType()
        {
            // type ((True 1) (False 0))
            var term =
                Term.DiscriminatedUnionType(
                    Term.Bind(
                        Term.Identity("True"),
                        Term.Identity("True0")),
                    Term.Bind(
                        Term.Identity("False"),
                        Term.Identity("False0")));

            var environment = Environment.Create();
            var inferred = environment.Infer(term);
            var du = (DiscriminatedUnionTypeTerm)environment.Reduce(inferred);

            var True = environment.LookupBoundTerm("True");

            //Assert.AreEqual(, actual.Constructors[0]);
        }
    }
}
