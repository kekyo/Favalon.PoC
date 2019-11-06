using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class TransposeTest
    {
        [Test]
        public void TransposeNonTransposableTokens()
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            Assert.AreEqual(
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi")),
                actual);
        }
    }
}
