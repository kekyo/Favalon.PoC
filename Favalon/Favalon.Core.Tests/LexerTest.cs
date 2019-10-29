using Favalon.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class LexerTest
    {
        private static readonly Func<string, IEnumerable<Token>>[] LexerRunners =
            new[]
            {
                new Func<string, IEnumerable<Token>>(text => Lexer.EnumerableTokens(text)),
                new Func<string, IEnumerable<Token>>(text => Lexer.EnumerableTokens(text.AsEnumerable())),
                new Func<string, IEnumerable<Token>>(text => Lexer.EnumerableTokens(new StringReader(text))),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "  abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc def ghi  ";
            var actual = run(text);

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensLongSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc      def      ghi";
            var actual = run(text);

            Assert.AreEqual(
                new[] { Token.Identity("abc"), Token.Identity("def"), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "(abc def) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Begin(), Token.Identity("abc"), Token.Identity("def"), Token.End(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc (def ghi)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.Begin(), Token.Identity("def"), Token.Identity("ghi"), Token.End() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc ( def ) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.Begin(), Token.Identity("def"), Token.End(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithNoSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc(def)ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.Begin(), Token.Identity("def"), Token.End(), Token.Identity("ghi") },
                actual);
        }
    }
}
