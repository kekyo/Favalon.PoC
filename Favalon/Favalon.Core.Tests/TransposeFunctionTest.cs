using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class TransposeFunctionTest
    {
        [Test]
        public void TransposeArrowOperator()
        {
            // -> a b
            var term = Term.Apply(
                Term.Apply(
                    Term.Identity("->"),
                    Term.Identity("a")),
                Term.Identity("b"));

            var environment = Environment.Create();
            var actual = environment.TransposeFunction(term);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("a"),
                    Term.Identity("b")),
                actual);
        }
    }
}
