using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class ApplyTermTest
    {
        [Test]
        public void Apply()
        {
            var actual = Term.Apply(
                Term.Variable("x"),
                Term.Variable("y"));

            Assert.AreEqual("x y", actual.ToString());
        }

        [Test]
        public void ReplaceAtFunction()
        {
            var a = Term.Apply(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = a.VisitReplace("x", Term.Variable("z"));

            Assert.AreEqual("z y", actual.ToString());
        }

        [Test]
        public void ReplaceAtArgument()
        {
            var a = Term.Apply(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = a.VisitReplace("y", Term.Variable("z"));

            Assert.AreEqual("x z", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var a = Term.Apply(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = a.VisitReplace("z", Term.Variable("q"));

            Assert.AreEqual("x y", actual.ToString());
        }

        [Test]
        public void ReduceNotReduceable()
        {
            var a = Term.Apply(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = a.Reduce();

            Assert.AreEqual("x y", actual.ToString());
        }

        [Test]
        public void ReduceContainsFunction()
        {
            // (x -> x) y
            var a = Term.Apply(
                Term.Function(
                    Term.Variable("x"),
                    Term.Variable("x")),
                Term.Variable("y"));
            var actual = a.Reduce();

            Assert.AreEqual("y", actual.ToString());
        }
    }
}
