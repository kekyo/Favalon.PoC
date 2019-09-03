using Favalon.Tokens;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class LexerTest
    {
        [Test]
        public void EmptyString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(string.Empty).ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void WhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void WhiteSpacesString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("  ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void TabbedString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\t").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void TabbedAndWhiteSpacesString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\t  \t ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void NumericString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("123").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken("123"),
                }, tokens);
        }

        public static readonly char[] NumericChars = "0123456789".ToArray();

        [TestCaseSource("NumericChars")]
        public void NumericsString(char inch)
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(inch.ToString()).ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken(inch.ToString()),
                }, tokens);
        }

        [Test]
        public void NumericWithOperatorsString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("123+456-789").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken("123"),
                    new OperatorToken("+"),
                    new NumericToken("456"),
                    new OperatorToken("-"),
                    new NumericToken("789"),
                }, tokens);
        }

        [Test]
        public void NumericBeforeWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" 123").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken("123"),
                }, tokens);
        }

        [Test]
        public void NumericAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("123 ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken("123"),
                }, tokens);
        }

        [Test]
        public void NumericBeforeAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" 123 ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new NumericToken("123"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void OperatorString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("+").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken("+"),
                }, tokens);
        }

        [Test]
        public void OperatorsString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(Lexer.OperatorChars).ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken(Lexer.OperatorChars),
                }, tokens);
        }

        public static readonly char[] OperatorChars = Lexer.OperatorChars.ToArray();

        [TestCaseSource("OperatorChars")]
        public void OperatorCharsString(char inch)
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(inch.ToString()).ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken(inch.ToString()),
                }, tokens);
        }

        [Test]
        public void OperatorBeforeWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" +").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken("+"),
                }, tokens);
        }

        [Test]
        public void OperatorAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("+ ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken("+"),
                }, tokens);
        }

        [Test]
        public void OperatorBeforeAndAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" + ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new OperatorToken("+"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void StringString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"123\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("123"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString1()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\" 123\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken(" 123"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString2()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"1 23\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("1 23"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString3()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"123 \"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("123 "),
                }, tokens);
        }

        [Test]
        public void StringWithOperatorsString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"abc\"+\"def\"-\"ghi\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("abc"),
                    new OperatorToken("+"),
                    new StringToken("def"),
                    new OperatorToken("-"),
                    new StringToken("ghi"),
                }, tokens);
        }

        [Test]
        public void StringWithVariablesString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"abc\"aaa\"def\"bbb\"ghi\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("abc"),
                    new VariableToken("aaa"),
                    new StringToken("def"),
                    new VariableToken("bbb"),
                    new StringToken("ghi"),
                }, tokens);
        }

        [Test]
        public void StringBeforeWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" \"123\"").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("123"),
                }, tokens);
        }

        [Test]
        public void StringAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("\"123\" ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("123"),
                }, tokens);
        }

        [Test]
        public void StringBeforeAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" \"123\" ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new StringToken("123"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void VariableString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("abc").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new VariableToken("abc"),
                }, tokens);
        }

        [Test]
        public void VariableWithNumericString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("a1b2c3").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new VariableToken("a1b2c3"),
                }, tokens);
        }

        [Test]
        public void VariableWithOperatorsString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("abc+def-ghi").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new VariableToken("abc"),
                    new OperatorToken("+"),
                    new VariableToken("def"),
                    new OperatorToken("-"),
                    new VariableToken("ghi"),
                }, tokens);
        }

        [Test]
        public void VariableBeforeWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex(" abc").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new VariableToken("abc"),
                }, tokens);
        }

        [Test]
        public void VariableAfterWhiteSpaceString()
        {
            var lexer = new Lexer();

            var tokens = lexer.Lex("abc ").ToArray();

            Assert.AreEqual(
                new Token[]
                {
                    new VariableToken("abc"),
                }, tokens);
        }
    }
}
