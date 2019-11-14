using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class IdentityTermTest
    {
        [Test]
        public void Identity()
        {
            var actual = Term.Identity("x");

            Assert.AreEqual("x", actual.Readable);
        }

        [Test]
        public void Replace()
        {
            var id = Term.Identity("x");

            var environment = Environment.Create();
            var actual = environment.Replace(id, "x", Term.Identity("y"));

            Assert.AreEqual("y", actual.Readable);
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var id = Term.Identity("x");

            var environment = Environment.Create();
            var actual = environment.Replace(id, "z", Term.Identity("y"));

            Assert.AreEqual("x", actual.Readable);
        }

        [Test]
        public void Reduce()
        {
            var id = Term.Identity("x");

            var environment = Environment.Create();
            var actual = environment.Reduce(id);

            Assert.AreEqual("x", actual.Readable);
        }
    }
}
