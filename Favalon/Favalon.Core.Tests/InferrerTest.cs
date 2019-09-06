using Favalon.Expressions;
using Favalon.Terms;
using Favalon.Tokens;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class InferrerTest
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

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

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

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

            Assert.AreEqual(
                new BooleanExpression(false),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferString()
        {
            var term = Parse("\"abc\"");

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

            Assert.AreEqual(
                new StringExpression("abc"),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferVariableForString()
        {
            var term = Parse("abc");

            var interrer = new Inferrer();
            interrer.Add("abc", new StringExpression("abc"));

            var expression = interrer.Infer(term);

            Assert.AreEqual(
                new StringExpression("abc"),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferVariableWithAnnotation()
        {
            var term = Parse("abc:System.Boolean");

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

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

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

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

            var interrer = new Inferrer();
            interrer.Add("System.Boolean", TypeExpression<bool>.Instance);

            var expression = interrer.Infer(term);

            Assert.AreEqual(
                new VariableExpression(
                    "abc",
                    TypeExpression<bool>.Instance),
                expression);
        }

        ////////////////////////////////////////////////////////////////////////

        [Test]
        public void InferLambda()
        {
            var term = Parse("-> abc def");

            var interrer = new Inferrer();
            var expression = interrer.Infer(term);

            // App(App(Var(->) Var(abc)) Var(def))
            // Lambda(abc def)

            Assert.AreEqual(
                new LambdaExpression(
                    new BoundExpression(
                        "abc",
                        UnspecifiedExpression.Instance),
                    new VariableExpression(
                        "def",
                        UnspecifiedExpression.Instance)),
                expression);
        }
    }
}
