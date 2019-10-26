using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class OverallTest
    {
        // Church numeral calculas
        private static readonly Term one =
            Term.Function(
                Term.Identity("p"),
                Term.Function(
                    Term.Identity("x"),
                    Term.Apply(
                        Term.Identity("p"),
                        Term.Identity("x"))));

        private static readonly Term increment =
            Term.Function(
                Term.Identity("n"),
                Term.Function(
                    Term.Identity("p"),
                    Term.Function(
                        Term.Identity("x"),
                        Term.Apply(
                            Term.Identity("p"),
                            Term.Apply(
                                Term.Apply(
                                    Term.Identity("n"),
                                    Term.Identity("p")),
                                Term.Identity("x"))))));

        private static readonly Term add =
            Term.Function(
                Term.Identity("m"),
                Term.Function(
                    Term.Identity("n"),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("n"),
                            increment),
                        Term.Identity("m"))));

        [Test]
        public void OneTerm()
        {
            Assert.AreEqual("p -> x -> p x", one.ToString());
        }

        [Test]
        public void IncrementTerm()
        {
            Assert.AreEqual("n -> p -> x -> p (n p x)", increment.ToString());
        }

        [Test]
        public void AddTerm()
        {
            Assert.AreEqual("m -> n -> n (n -> p -> x -> p (n p x)) m", add.ToString());
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
            var reduced = add2Times.Reduce();

            Assert.AreEqual("p -> x -> p ((p -> x -> p x) p x)", reduced.ToString());

            var inc = Term.Identity("inc");
            var zero = Term.Identity("zero");

            var final =
                Term.Apply(
                    Term.Apply(
                        reduced,
                        inc),
                    zero);
            var actual = final.Reduce();

            Assert.AreEqual("inc (inc zero)", actual.ToString());
        }
    }
}
