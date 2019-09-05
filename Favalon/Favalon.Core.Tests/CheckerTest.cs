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

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferTrue()
        {
            var term = Parse("true");

            var checker = new Checker();
            var expression = checker.Infer(term);

            Assert.AreEqual(
                new BooleanExpression(true),
                expression);
            Assert.AreEqual(
                TypeExpression<bool>.Instance,
                expression.HigherOrder);
        }

        [Test]
        public void InferFalse()
        {
            var term = Parse("false");

            var checker = new Checker();
            var expression = checker.Infer(term);

            Assert.AreEqual(
                new BooleanExpression(false),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferString()
        {
            var term = Parse("\"abc\"");

            var checker = new Checker();
            var expression = checker.Infer(term);

            Assert.AreEqual(
                new StringExpression("abc"),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferVariableForString()
        {
            var term = Parse("abc");

            var checker = new Checker();
            checker.Add("abc", new StringExpression("abc"));

            var expression = checker.Infer(term);

            Assert.AreEqual(
                new StringExpression("abc"),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferVariableWithAnnotation()
        {
            var term = Parse("abc:System.Boolean");

            var checker = new Checker();
            var expression = checker.Infer(term);

            Assert.AreEqual(
                new VariableExpression(
                    "abc",
                    new VariableExpression(
                        "System.Boolean",
                        UnspecifiedExpression.Instance)),
                expression);
        }

        [Test]
        public void InferVariableWithAnnotations()
        {
            var term = Parse("abc:System.Boolean:Kind");

            var checker = new Checker();
            var expression = checker.Infer(term);

            Assert.AreEqual(
                new VariableExpression(
                    "abc",
                    new VariableExpression(
                        "System.Boolean",
                        new VariableExpression(
                            "Kind",
                            UnspecifiedExpression.Instance))),
                expression);
        }

        [Test]
        public void InferVariableWithBoundAnnotation()
        {
            var term = Parse("abc:System.Boolean");

            var checker = new Checker();
            checker.Add("System.Boolean", TypeExpression<bool>.Instance);

            var expression = checker.Infer(term);

            Assert.AreEqual(
                new VariableExpression(
                    "abc",
                    TypeExpression<bool>.Instance),
                expression);
        }
    }
}
