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

            Assert.AreEqual("x", actual.ToString());
        }

        [Test]
        public void Replace()
        {
            var id = Term.Identity("x");
            var actual = id.VisitReplace("x", Term.Identity("y"));

            Assert.AreEqual("y", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var id = Term.Identity("x");
            var actual = id.VisitReplace("z", Term.Identity("y"));

            Assert.AreEqual("x", actual.ToString());
        }

        [Test]
        public void Reduce()
        {
            var id = Term.Identity("x");
            var actual = id.Reduce();

            Assert.AreEqual("x", actual.ToString());
        }
    }
}
