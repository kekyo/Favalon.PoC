using Favalon.Internal;
using Favalon.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "  abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc def ghi  ";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Identity("ghi"),
                    Token.WhiteSpace() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensLongSpace(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc      def      ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "(abc def) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Open(),
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.Close(), 
                    Token.WhiteSpace(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc (def ghi)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Open(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Identity("ghi"),
                    Token.Close() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc ( def ) ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Open(),
                    Token.WhiteSpace(),
                    Token.Identity("def"),
                    Token.WhiteSpace(),
                    Token.Close(),
                    Token.WhiteSpace(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithNoSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc(def)ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.Open(), 
                    Token.Identity("def"),
                    Token.Close(),
                    Token.Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTrailsNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "a12 d34 g56";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("a12"),
                    Token.WhiteSpace(),
                    Token.Identity("d34"),
                    Token.WhiteSpace(),
                    Token.Identity("g56") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("+"),
                    Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "-abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("-"),
                    Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "++abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("++"),
                    Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "--abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("--"),
                    Token.Identity("abc") },
                actual);
        }

        ///////////////////////////////////////////////

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 456 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombinedIdentityAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc 456 def";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Identity("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensBeforeBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "(123 456) 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Open(),
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.Close(),
                    Token.WhiteSpace(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensAfterBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 (456 789)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Open(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Numeric("789"),
                    Token.Close() },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 ( 456 ) 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Open(),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Close(),
                    Token.WhiteSpace(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithNoSpacingBrackets(Func<string, IEnumerable<Token>> run)
        {
            var text = "123(456)789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.Open(),
                    Token.Numeric("456"),
                    Token.Close(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusSignNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "+123 +456 +789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("+"),
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator("+"), 
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Operator("+"),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusSignNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "-123 -456 -789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("-"),
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator("-"),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Operator("-"),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "+ 123 + 456 + 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("+"),
                    Token.WhiteSpace(),
                    Token.Numeric("123"), 
                    Token.WhiteSpace(),
                    Token.Operator("+"),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Operator("+"),
                    Token.WhiteSpace(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "- 123 - 456 - 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("-"),
                    Token.WhiteSpace(),
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator("-"),
                    Token.WhiteSpace(),
                    Token.Numeric("456"),
                    Token.WhiteSpace(),
                    Token.Operator("-"),
                    Token.WhiteSpace(),
                    Token.Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorWithSpaceAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 + 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator("+"),
                    Token.WhiteSpace(),
                    Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorWithSpaceAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123 - 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator("-"),
                    Token.WhiteSpace(),
                    Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorSideBySideAndNumericTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "123+456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.Operator("+"),
                    Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorSideBySideAndNumericTokens(Func<string, IEnumerable<Token>> run)
        {
            var text = "123-456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.Operator("-"),
                    Token.Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableComplexNumericOperatorTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "-123*(+456+789)";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("-"),
                    Token.Numeric("123"),
                    Token.Operator("*"),
                    Token.Open(),
                    Token.Operator("+"),
                    Token.Numeric("456"),
                    Token.Operator("+"),
                    Token.Numeric("789"),
                    Token.Close()
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
                    Token.Operator("+"),
                    Token.Numeric("123"),
                    Token.Operator("*"),
                    Token.Open(),
                    Token.Operator("-"),
                    Token.Numeric("456"),
                    Token.Operator("-"),
                    Token.Numeric("789"),
                    Token.Close()
                },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens1(Func<string, IEnumerable<Token>> run)
        {
            var text = "++123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("++"),
                    Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens2(Func<string, IEnumerable<Token>> run)
        {
            var text = "--123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Operator("--"),
                    Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineIdentityAndNumericTokensWithOperator(Func<string, IEnumerable<Token>> run)
        {
            var text = "abc+123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Identity("abc"),
                    Token.Operator("+"),
                    Token.Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineNumericAndIdentityTokensWithOperator(Func<string, IEnumerable<Token>> run)
        {
            var text = "123+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Token.Numeric("123"),
                    Token.Operator("+"),
                    Token.Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void Operator1Detection(Func<string, IEnumerable<Token>> run)
        {
            foreach (var ch in Utilities.OperatorChars)
            {
                var text = $"123 {ch} abc";
                var actual = run(text);

                Assert.AreEqual(
                    new Token[] {
                    Token.Numeric("123"),
                    Token.WhiteSpace(),
                    Token.Operator(ch.ToString()),
                    Token.WhiteSpace(),
                    Token.Identity("abc") },
                    actual);
            }
        }

        [TestCaseSource("LexerRunners")]
        public void Operator2Detection(Func<string, IEnumerable<Token>> run)
        {
            Parallel.ForEach(
                Utilities.OperatorChars.
                    SelectMany(ch1 => Utilities.OperatorChars.
                        Select(ch2 => (ch1, ch2))),
                entry =>
                {
                    var text = $"123 {entry.ch1}{entry.ch2} abc";
                    var actual = run(text);

                    Assert.AreEqual(
                        new Token[] {
                        Token.Numeric("123"),
                        Token.WhiteSpace(),
                        Token.Operator($"{entry.ch1}{entry.ch2}"),
                        Token.WhiteSpace(),
                        Token.Identity("abc") },
                        actual);
                });
        }

        [TestCaseSource("LexerRunners")]
        public void Operator3Detection(Func<string, IEnumerable<Token>> run)
        {
            Parallel.ForEach(
                Utilities.OperatorChars.
                    SelectMany(ch1 => Utilities.OperatorChars.
                        SelectMany(ch2 => Utilities.OperatorChars.
                            Select(ch3 => (ch1, ch2, ch3)))),
                entry =>
                {
                    var text = $"123 {entry.ch1}{entry.ch2}{entry.ch3} abc";
                    var actual = run(text);

                    Assert.AreEqual(
                        new Token[] {
                        Token.Numeric("123"),
                        Token.WhiteSpace(),
                        Token.Operator($"{entry.ch1}{entry.ch2}{entry.ch3}"),
                        Token.WhiteSpace(),
                        Token.Identity("abc") },
                        actual);
                });
        }
    }
}
