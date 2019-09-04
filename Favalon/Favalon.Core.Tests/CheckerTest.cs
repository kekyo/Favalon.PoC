using Favalon.Expressions;
using Favalon.Terms;
using Favalon.Tokens;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class CheckerTest
    {
        private Term Parse(string text)
        {
            var lexer = new Lexer();
            var parser = new Parser();
            return parser.Parse(lexer.Tokenize(text)).Single();
        }

        [Test]
        public void InferTrue()
        {
            var term = Parse("true");

            var checker = new Checker();
            var expressions = checker.Infer(term);

            Assert.AreEqual(
                new[]
                {
                    new BooleanExpression(true)
                },
                expressions);
        }
    }
}
