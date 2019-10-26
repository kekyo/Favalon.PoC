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
                Term.Identity("x"),
                Term.Identity("y"));

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void ReplaceAtBody()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = f.VisitReplace("y", Term.Identity("z"));

            Assert.AreEqual("x -> z", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicableAtParameterIsVariable()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = f.VisitReplace("x", Term.Identity("z"));

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void Reduce()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = f.Reduce();

            Assert.AreEqual("x -> y", actual.ToString());
        }

        [Test]
        public void Call()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Function(
                    Term.Identity("y"),
                    Term.Apply(
                        Term.Identity("x"),
                        Term.Identity("y"))));
            var arg = Term.Function(
                Term.Identity("z"),
                Term.Identity("z"));
            var actual = f.Call(arg);

            Assert.AreEqual("y -> (z -> z) y", actual.ToString());
        }
    }
}
