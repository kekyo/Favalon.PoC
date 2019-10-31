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
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Identity("def"), Token.WhiteSpace(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "  abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Identity("def"), Token.WhiteSpace(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc def ghi  ";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Identity("def"), Token.WhiteSpace(), Token.Identity("ghi"), Token.WhiteSpace() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensLongSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc      def      ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Identity("def"), Token.WhiteSpace(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "(abc def) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Begin(), Token.Identity("abc"), Token.WhiteSpace(), Token.Identity("def"), Token.End(), Token.WhiteSpace(), Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc (def ghi)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Begin(), Token.Identity("def"), Token.WhiteSpace(), Token.Identity("ghi"), Token.End() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc ( def ) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Begin(), Token.WhiteSpace(), Token.Identity("def"), Token.WhiteSpace(), Token.End(), Token.WhiteSpace(), Token.Identity("ghi") },
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

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTrailsNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "a12 d34 g56";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("a12"), Token.WhiteSpace(), Token.Identity("d34"), Token.WhiteSpace(), Token.Identity("g56") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("+"), Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "-abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("-"), Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "++abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("++"), Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "--abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("--"), Token.Identity("abc") },
                actual);
        }

        ///////////////////////////////////////////////

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 456 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.WhiteSpace(), Token.Numeric("456"), Token.WhiteSpace(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombinedIdentityAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc 456 def";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.WhiteSpace(), Token.Numeric("456"), Token.WhiteSpace(), Token.Identity("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensBeforeBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "(123 456) 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Begin(), Token.Numeric("123"), Token.WhiteSpace(), Token.Numeric("456"), Token.End(), Token.WhiteSpace(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensAfterBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 (456 789)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.WhiteSpace(), Token.Begin(), Token.Numeric("456"), Token.WhiteSpace(), Token.Numeric("789"), Token.End() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 ( 456 ) 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.WhiteSpace(), Token.Begin(), Token.WhiteSpace(), Token.Numeric("456"), Token.WhiteSpace(), Token.End(), Token.WhiteSpace(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithNoSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123(456)789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.Begin(), Token.Numeric("456"), Token.End(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusSignNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "+123 +456 +789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("+"), Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("+"), Token.Numeric("456"), Token.WhiteSpace(), Token.Identity("+"), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusSignNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "-123 -456 -789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("-"), Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("-"), Token.Numeric("456"), Token.WhiteSpace(), Token.Identity("-"), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "+ 123 + 456 + 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("+"), Token.WhiteSpace(), Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("+"), Token.WhiteSpace(), Token.Numeric("456"), Token.WhiteSpace(), Token.Identity("+"), Token.WhiteSpace(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "- 123 - 456 - 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("-"), Token.WhiteSpace(), Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("-"), Token.WhiteSpace(), Token.Numeric("456"), Token.WhiteSpace(), Token.Identity("-"), Token.WhiteSpace(), Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorWithSpaceAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 + 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("+"), Token.WhiteSpace(), Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorWithSpaceAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 - 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.WhiteSpace(), Token.Identity("-"), Token.WhiteSpace(), Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorSideBySideAndNumericTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "123+456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.Identity("+"), Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorSideBySideAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123-456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.Identity("-"), Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableComplexNumericOperatorTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "-123*(+456+789)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("-"),
                    Token.Numeric("123"),
                    Token.Identity("*"),
                    Token.Begin(),
                    Token.Identity("+"),
                    Token.Numeric("456"),
                    Token.Identity("+"),
                    Token.Numeric("789"),
                    Token.End()
                },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableComplexNumericOperatorTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "+123*(-456-789)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("+"),
                    Token.Numeric("123"),
                    Token.Identity("*"),
                    Token.Begin(),
                    Token.Identity("-"),
                    Token.Numeric("456"),
                    Token.Identity("-"),
                    Token.Numeric("789"),
                    Token.End()
                },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "++123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("++"), Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "--123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("--"), Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineIdentityAndNumericTokensWithOperator(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc+123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Identity("abc"), Token.Identity("+"), Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineNumericAndIdentityTokensWithOperator(Func<string, IEnumerable<Token>> run)
        {
            var text = "123+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] { Token.Numeric("123"), Token.Identity("+"), Token.Identity("abc") },
                actual);
        }
    }
}
