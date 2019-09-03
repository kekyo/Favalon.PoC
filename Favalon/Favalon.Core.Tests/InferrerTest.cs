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
    }
}
