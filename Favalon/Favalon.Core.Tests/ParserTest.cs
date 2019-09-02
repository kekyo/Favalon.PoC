using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void EmptyString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(string.Empty).ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void WhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void WhiteSpacesString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("  ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void TabbedString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\t").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        [Test]
        public void TabbedAndWhiteSpacesString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\t  \t ").ToArray();

            Assert.AreEqual(0, tokens.Length);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void NumericString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("123").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, "123"),
                }, tokens);
        }

        public static readonly char[] NumericChars = "0123456789".ToArray();

        [TestCaseSource("NumericChars")]
        public void NumericsString(char inch)
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(inch.ToString()).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, inch.ToString()),
                }, tokens);
        }

        [Test]
        public void NumericWithOperatorsString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("123+456-789").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, "123"),
                    new Token(TokenTypes.Variable, "+"),
                    new Token(TokenTypes.Numeric, "456"),
                    new Token(TokenTypes.Variable, "-"),
                    new Token(TokenTypes.Numeric, "789"),
                }, tokens);
        }

        [Test]
        public void NumericBeforeWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" 123").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, "123"),
                }, tokens);
        }

        [Test]
        public void NumericAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("123 ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, "123"),
                }, tokens);
        }

        [Test]
        public void NumericBeforeAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" 123 ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Numeric, "123"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void OperatorString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("+").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "+"),
                }, tokens);
        }

        [Test]
        public void OperatorsString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(Parser.OperatorChars).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, Parser.OperatorChars),
                }, tokens);
        }

        public static readonly char[] OperatorChars = Parser.OperatorChars.ToArray();

        [TestCaseSource("OperatorChars")]
        public void OperatorCharsString(char inch)
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(inch.ToString()).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, inch.ToString()),
                }, tokens);
        }

        [Test]
        public void OperatorBeforeWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" +").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "+"),
                }, tokens);
        }

        [Test]
        public void OperatorAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("+ ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "+"),
                }, tokens);
        }

        [Test]
        public void OperatorBeforeAndAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" + ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "+"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void StringString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\"123\"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "123"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString1()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\" 123\"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, " 123"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString2()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\"1 23\"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "1 23"),
                }, tokens);
        }

        [Test]
        public void StringWithWhiteSpaceString3()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\"123 \"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "123 "),
                }, tokens);
        }

        [Test]
        public void StringWithOperatorsString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\"abc\"+\"def\"-\"ghi\"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "abc"),
                    new Token(TokenTypes.Variable, "+"),
                    new Token(TokenTypes.String, "def"),
                    new Token(TokenTypes.Variable, "-"),
                    new Token(TokenTypes.String, "ghi"),
                }, tokens);
        }

        [Test]
        public void StringBeforeWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" \"123\"").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "123"),
                }, tokens);
        }

        [Test]
        public void StringAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("\"123\" ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "123"),
                }, tokens);
        }

        [Test]
        public void StringBeforeAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" \"123\" ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.String, "123"),
                }, tokens);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void VariableString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("abc").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "abc"),
                }, tokens);
        }

        [Test]
        public void VariableWithNumericString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("a1b2c3").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "a1b2c3"),
                }, tokens);
        }

        [Test]
        public void VariableWithOperatorsString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("abc+def-ghi").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "abc"),
                    new Token(TokenTypes.Variable, "+"),
                    new Token(TokenTypes.Variable, "def"),
                    new Token(TokenTypes.Variable, "-"),
                    new Token(TokenTypes.Variable, "ghi"),
                }, tokens);
        }

        [Test]
        public void VariableBeforeWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize(" abc").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "abc"),
                }, tokens);
        }

        [Test]
        public void VariableAfterWhiteSpaceString()
        {
            var parser = new Parser();

            var tokens = parser.Tokenize("abc ").ToArray();

            Assert.AreEqual(
                new[]
                {
                    new Token(TokenTypes.Variable, "abc"),
                }, tokens);
        }
    }
}
