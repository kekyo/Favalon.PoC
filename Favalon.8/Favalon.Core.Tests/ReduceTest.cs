using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class ReduceTest
    {
        // Church numeral calculas
        private static readonly Term one =
            Term.Function(
                Term.BoundIdentity("p"),
                Term.Function(
                    Term.BoundIdentity("x"),
                    Term.Apply(
                        Term.Identity("p"),
                        Term.Identity("x"))));

        private static readonly Term increment =
            Term.Function(
                Term.BoundIdentity("n"),
                Term.Function(
                    Term.BoundIdentity("p"),
                    Term.Function(
                        Term.BoundIdentity("x"),
                        Term.Apply(
                            Term.Identity("p"),
                            Term.Apply(
                                Term.Apply(
                                    Term.Identity("n"),
                                    Term.Identity("p")),
                                Term.Identity("x"))))));

        private static readonly Term add =
            Term.Function(
                Term.BoundIdentity("m"),
                Term.Function(
                    Term.BoundIdentity("n"),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("n"),
                            increment),
                        Term.Identity("m"))));

        [Test]
        public void OneTerm()
        {
            Assert.AreEqual("p -> x -> p x", one.Readable);
        }

        [Test]
        public void IncrementTerm()
        {
            Assert.AreEqual("n -> p -> x -> p (n p x)", increment.Readable);
        }

        [Test]
        public void AddTerm()
        {
            Assert.AreEqual("m -> n -> n (n -> p -> x -> p (n p x)) m", add.Readable);
        }

        [Test]
        public void Reduce()
        {
            // + 1 1
            var add2Times =
                Term.Apply(
                    Term.Apply(
                        add,
                        one),
                    one);

            var environment = Environment.Create();
            var reduced = environment.EnumerableReduceSteps(add2Times).
                ToArray();

            Assert.AreEqual("p -> x -> p ((p -> x -> p x) p x)", reduced.Last().Readable);

            var inc = Term.Identity("inc");
            var zero = Term.Identity("zero");

            var final =
                Term.Apply(
                    Term.Apply(
                        reduced.Last(),
                        inc),
                    zero);

            var actual = environment.Reduce(final);

            Assert.AreEqual("inc (inc zero)", actual.Readable);
        }
    }
}
