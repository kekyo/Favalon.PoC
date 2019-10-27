using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class OverallTest
    {
        [Test]
        public void EnumerableIdentityToken()
        {
            var text = "(-> x x x) -> y y";
            var tokens = Lexer.EnumerableTokens(new StringReader(text)).ToArray();
            var term = Parser.EnumerableTerms(tokens).
                Single();
            var actual = term.Reduce();

            Assert.AreEqual(
                Term.Apply(
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y")),
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y"))),
                actual);
        }
    }
}
