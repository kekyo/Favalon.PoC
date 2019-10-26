using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void EnumerableIdentityToken()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Identity("abc"),
                });

            Assert.AreEqual(
                new[] { Term.Identity("abc") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokens()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableArrowToken()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Identity("abc"),
                    Token.Identity("->"),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("->")), Term.Identity("ghi")) },
                actual);
        }
    }
}
