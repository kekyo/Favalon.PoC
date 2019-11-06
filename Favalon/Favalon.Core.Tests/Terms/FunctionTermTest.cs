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

            Assert.AreEqual("x -> y", actual.Readable);
        }

        [Test]
        public void ReplaceAtBody()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Replace(f, "y", Term.Identity("z"));

            Assert.AreEqual("x -> z", actual.Readable);
        }

        [Test]
        public void ReplaceNotApplicableAtParameterIsVariable()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Replace(f, "x", Term.Identity("z"));

            Assert.AreEqual("x -> y", actual.Readable);
        }

        [Test]
        public void Reduce()
        {
            var f = Term.Function(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Reduce(f);

            Assert.AreEqual("x -> y", actual.Readable);
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

            var environment = Environment.Create();
            var actual = environment.Call(f, arg);

            Assert.AreEqual("y -> (z -> z) y", actual.Readable);
        }
    }
}
