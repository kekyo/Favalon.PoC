using Favalon.Tokens;
using NUnit.Framework;
using System.IO;

namespace Favalon
{
    [TestFixture]
    public sealed class LexerTest
    {
        [Test]
        public void EnumerableIdentityTokens()
        {
            var text = "abc def ghi";
            var actual = Lexer.EnumerableTokens(new StringReader(text));

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokensBeforeSpace()
        {
            var text = "  abc def ghi";
            var actual = Lexer.EnumerableTokens(new StringReader(text));

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokensAfterSpace()
        {
            var text = "abc def ghi  ";
            var actual = Lexer.EnumerableTokens(new StringReader(text));

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokensLongSpace()
        {
            var text = "abc      def      ghi";
            var actual = Lexer.EnumerableTokens(new StringReader(text));

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }
    }
}
