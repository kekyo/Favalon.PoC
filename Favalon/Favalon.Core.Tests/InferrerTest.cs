using Favalon.Terms;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon
{
    [TestFixture]
    public sealed class InferrerTest
    {
        private Token[] Parse(string text)
        {
            var parser = new Parser();
            return parser.Tokenize(text).ToArray();
        }

        [Test]
        public void InferTrue()
        {
            var tokens = Parse("true");

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new BooleanTerm(true)
                },
                terms);
        }

        [Test]
        public void InferFalse()
        {
            var tokens = Parse("false");

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new BooleanTerm(false)
                },
                terms);
        }

        [TestCase("false true", new[] { false, true })]
        [TestCase("true false", new[] { true, false })]
        public void InferBooleanValues(string args, bool[] expected)
        {
            var tokens = Parse(args);

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                expected.Select(v => new BooleanTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferNumeric()
        {
            var tokens = Parse("123");

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new NumericTerm("123")
                },
                terms);
        }

        [TestCase("123 456", new[] { "123", "456" })]
        public void InferNumericValues(string args, string[] expected)
        {
            var tokens = Parse(args);

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                expected.Select(v => new NumericTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferString()
        {
            var tokens = Parse("\"abc\"");

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new StringTerm("abc")
                },
                terms);
        }

        [TestCase("\"abc\" \"def\"", new[] { "abc", "def" })]
        public void InferStringValues(string args, string[] expected)
        {
            var tokens = Parse(args);

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                expected.Select(v => new StringTerm(v)),
                terms);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferVariable()
        {
            var tokens = Parse("abc");

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new VariableTerm("abc")
                },
                terms);
        }

        public static char[] OperatorChars = Parser.OperatorChars.ToArray();

        [TestCaseSource("OperatorChars")]
        public void InferOperatorChars(char inch)
        {
            var tokens = Parse(inch.ToString());

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                new[]
                {
                    new VariableTerm(inch.ToString())
                },
                terms);
        }

        [TestCase("abc def", new[] { "abc", "def" })]
        [TestCase("abc+d1e2f3-ghi", new[] { "abc", "+", "d1e2f3", "-", "ghi" })]
        public void InferVariables(string args, string[] expected)
        {
            var tokens = Parse(args);

            var inferrer = new Inferrer();
            var terms = inferrer.Infer(tokens);

            Assert.AreEqual(
                expected.Select(v => new VariableTerm(v)),
                terms);
        }
    }
}
