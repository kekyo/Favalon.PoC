using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class InferTest
    {
        [Test]
        public void ApplyArrow()
        {
            var a = Term.Apply(
                Term.Apply(
                    Term.Identity("->"),
                    Term.Identity("a")),
                Term.Identity("b"));
            var actual = a.Reduce();

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("a"),
                    Term.Identity("b")),
                actual);
        }
    }
}
