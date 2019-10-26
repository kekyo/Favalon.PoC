using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class VariableTermTest
    {
        [Test]
        public void Variable()
        {
            var actual = Term.Variable("x");

            Assert.AreEqual("x", actual.ToString());
        }

        [Test]
        public void Replace()
        {
            var v = Term.Variable("x");
            var actual = v.VisitReplace("x", Term.Variable("y"));

            Assert.AreEqual("y", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var v = Term.Variable("x");
            var actual = v.VisitReplace("z", Term.Variable("y"));

            Assert.AreEqual("x", actual.ToString());
        }

        [Test]
        public void Reduce()
        {
            var v = Term.Variable("x");
            var actual = v.Reduce();

            Assert.AreEqual("x", actual.ToString());
        }
    }
}
