using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class OverallTest
    {
        // Church numeral calculas
        private static readonly Term one =
            Term.Function(
                Term.Variable("p"),
                Term.Function(
                    Term.Variable("x"),
                    Term.Apply(
                        Term.Variable("p"),
                        Term.Variable("x"))));

        private static readonly Term increment =
            Term.Function(
                Term.Variable("n"),
                Term.Function(
                    Term.Variable("p"),
                    Term.Function(
                        Term.Variable("x"),
                        Term.Apply(
                            Term.Variable("p"),
                            Term.Apply(
                                Term.Apply(
                                    Term.Variable("n"),
                                    Term.Variable("p")),
                                Term.Variable("x"))))));

        private static readonly Term add =
            Term.Function(
                Term.Variable("m"),
                Term.Function(
                    Term.Variable("n"),
                    Term.Apply(
                        Term.Apply(
                            Term.Variable("n"),
                            increment),
                        Term.Variable("m"))));

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

            var inc = Term.Variable("inc");
            var zero = Term.Variable("zero");

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
