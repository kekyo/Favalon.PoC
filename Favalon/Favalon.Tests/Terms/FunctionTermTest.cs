using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class FunctionTermTest
    {
        [Test]
        public void Function()
        {
            var actual = Term.Function(
                Term.Variable("x"),
                Term.Variable("y"));

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void ReplaceAtBody()
        {
            var f = Term.Function(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = f.VisitReplace("y", Term.Variable("z"));

            Assert.AreEqual("x -> z", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicableAtParameterIsVariable()
        {
            var f = Term.Function(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = f.VisitReplace("x", Term.Variable("z"));

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void Reduce()
        {
            var f = Term.Function(
                Term.Variable("x"),
                Term.Variable("y"));
            var actual = f.Reduce();

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void Call()
        {
            var f = Term.Function(
                Term.Variable("x"),
                Term.Function(
                    Term.Variable("y"),
                    Term.Apply(
                        Term.Variable("x"),
                        Term.Variable("y"))));
            var arg = Term.Function(
                Term.Variable("z"),
                Term.Variable("z"));
            var actual = f.Call(arg);

            Assert.AreEqual("y -> (z -> z) y", actual.ToString());
        }
    }
}
