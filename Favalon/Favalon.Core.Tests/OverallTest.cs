using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class OverallTest
    {
        [Test]
        public void EnumerableIdentityToken1()
        {
            var text = "(-> x x) -> y y";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("y"),
                    Term.Identity("y")),
                actual);
        }

        [Test]
        public void EnumerableIdentityToken2()
        {
            var text = "(-> x x x) -> y y";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var actual1 = environment.Reduce(term, false);
            var actual2 = environment.Reduce(actual1, false);

            Assert.AreEqual(
                Term.Apply(
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y")),
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y"))),
                actual1);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("y"),
                    Term.Identity("y")),
                actual2);
        }
    }
}
