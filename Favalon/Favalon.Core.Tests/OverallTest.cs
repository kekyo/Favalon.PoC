using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class OverallTest
    {
        [Test]
        public void EnumerableIdentityToken1()
        {
            var text = "a -> b";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual = environment.Reduce(transposed);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("a"),
                    Term.Identity("b")),
                actual);
        }

        [Test]
        public void EnumerableIdentityToken2()
        {
            var text = "a -> b c";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual = environment.Reduce(transposed);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("a"),
                    Term.Apply(
                        Term.Identity("b"),
                        Term.Identity("c"))),
                actual);
        }

        [Test]
        public void EnumerableIdentityToken3()
        {
            var text = "-> a (b c)";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual = environment.Reduce(transposed);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("a"),
                    Term.Apply(
                        Term.Identity("b"),
                        Term.Identity("c"))),
                actual);
        }

        [Test]
        public void EnumerableIdentityToken4()
        {
            var text = "-> x (x x) ->";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual = environment.Reduce(transposed);

            Assert.AreEqual(
                Term.Apply(
                    Term.Function(
                        Term.Identity("a"),
                        Term.Apply(
                            Term.Identity("b"),
                            Term.Identity("c"))),
                    environment.LookupBoundTerms(Term.Operator("->"))![0].Term),
                actual);
        }

        //////////////////////////////////////////////////

        [Test]
        public void EnumerableIdentityToken11()
        {
            var text = "(-> x x) -> y y";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual = environment.Reduce(transposed);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("y"),
                    Term.Identity("y")),
                actual);
        }

        [Test]
        public void EnumerableIdentityToken12()
        {
            var text = "(-> x (x x)) -> y y";
            var tokens = Lexer.EnumerableTokens(text);
            var term = Parser.EnumerableTerms(tokens).
                Single();

            var environment = Environment.Create();
            var transposed = environment.Transpose(term);
            var actual1 = environment.Reduce(transposed, false);
            var actual2 = environment.Reduce(actual1, false);

            Assert.AreEqual(
                Term.Apply(
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y")),
                    Term.Function(
                        Term.Identity("y"),
                        Term.Identity("y"))),
                actual1);

            Assert.AreEqual(
                Term.Function(
                    Term.Identity("y"),
                    Term.Identity("y")),
                actual2);
        }
    }
}
