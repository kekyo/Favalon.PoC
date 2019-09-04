using Favalon.Terms;
using Favalon.Tokens;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        private Parser CreateParser() => new Parser();

        private Token[] Tokenize(string text)
        {
            var lexer = new Lexer();
            return lexer.Tokenize(text).ToArray();
        }

        [Test]
        public void ParseTrue()
        {
            var tokens = Tokenize("true");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new BooleanTerm(true)
                },
                terms);
        }

        [Test]
        public void ParseFalse()
        {
            var tokens = Tokenize("false");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new BooleanTerm(false)
                },
                terms);
        }

        [TestCase("false true", new[] { false, true })]
        [TestCase("true false", new[] { true, false })]
        public void ParseBooleanValues(string args, bool[] expected)
        {
            var tokens = Tokenize(args);

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                expected.Select(v => new BooleanTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void ParseNumeric()
        {
            var tokens = Tokenize("123");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new NumericTerm("123")
                },
                terms);
        }

        [TestCase("123 456", new[] { "123", "456" })]
        public void ParseNumericValues(string args, string[] expected)
        {
            var tokens = Tokenize(args);

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                expected.Select(v => new NumericTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void ParseString()
        {
            var tokens = Tokenize("\"abc\"");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new StringTerm("abc")
                },
                terms);
        }

        [TestCase("\"abc\" \"def\"", new[] { "abc", "def" })]
        public void ParseStringValues(string args, string[] expected)
        {
            var tokens = Tokenize(args);

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                expected.Select(v => new StringTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void ParseVariable()
        {
            var tokens = Tokenize("abc");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new VariableTerm("abc")
                },
                terms);
        }

        public static char[] OperatorChars = Lexer.OperatorChars.ToArray();

        [TestCaseSource("OperatorChars")]
        public void ParseOperatorChars(char inch)
        {
            var tokens = Tokenize(inch.ToString());

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new VariableTerm(inch.ToString())
                },
                terms);
        }

        [TestCase("abc def ghi", new[] { "abc", "def" }, "ghi")]
        [TestCase("abc+d1e2f3-ghi*jkl", new[] { "abc", "+", "d1e2f3", "-", "ghi" }, "jkl")]
        public void ParseVariables(string args, string[] expected, string expectedLast)
        {
            var tokens = Tokenize(args);

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            var expectedTerm =
                expected.
                Reverse().
                Aggregate(
                    (Term)new VariableTerm(expectedLast),
                    (term, v) => new ApplyTerm(v, term));

            Assert.AreEqual(
                new[] { expectedTerm },
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void ParseApply()
        {
            var tokens = Tokenize("a b");

            var parser = CreateParser();
            var terms = parser.Parse(tokens).ToArray();

            Assert.AreEqual(
                new[]
                {
                    new ApplyTerm("a", new VariableTerm("b"))
                },
                terms);
        }
    }
}
