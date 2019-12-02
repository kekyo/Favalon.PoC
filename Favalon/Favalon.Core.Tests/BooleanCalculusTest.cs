using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class BooleanCalculusTest
    {
        private static Term[] ParseAndReduce(params Token[] tokens)
        {
            var environment = Environment.Create();
            return environment.Parse(tokens).
                Select(term => environment.Reduce(term)).
                ToArray();
        }

        [Test]
        public void BooleanTrueSymbol()
        {
            var actual = ParseAndReduce(new[] {
                Token.Identity("true")
            });

            Assert.AreEqual(
                new[] {
                    Term.Constant(true) },
                actual);
        }

        [Test]
        public void BooleanFalseSymbol()
        {
            var actual = ParseAndReduce(new[] {
                Token.Identity("false")
            });

            Assert.AreEqual(
                new[] {
                    Term.Constant(false) },
                actual);
        }

        [TestCase(true, "true", "true")]
        [TestCase(false, "false", "true")]
        [TestCase(false, "true", "false")]
        [TestCase(false, "false", "false")]
        public void AndCalculus(bool result, string lhs, string rhs)
        {
            var actual = ParseAndReduce(new[] {
                Token.Identity(lhs),
                Token.Identity("&&"),
                Token.Identity(rhs),
            });

            Assert.AreEqual(
                new[] {
                    Term.Constant(result) },
                actual);
        }

        [TestCase(true, "true", "true")]
        [TestCase(true, "false", "true")]
        [TestCase(true, "true", "false")]
        [TestCase(false, "false", "false")]
        public void OrCalculus(bool result, string lhs, string rhs)
        {
            var actual = ParseAndReduce(new[] {
                Token.Identity(lhs),
                Token.Identity("||"),
                Token.Identity(rhs),
            });

            Assert.AreEqual(
                new[] {
                    Term.Constant(result) },
                actual);
        }

        [TestCase(true, "true", "&&", "true", "&&", "true")]
        [TestCase(true, "true", "||", "true", "||", "true")]
        [TestCase(true, "true", "&&", "true", "||", "true")]
        [TestCase(true, "true", "||", "true", "&&", "true")]
        public void CombinedAndOrCalculus(bool result, string lhs, string op1, string chs, string op2, string rhs)
        {
            var actual = ParseAndReduce(new[] {
                Token.Identity(lhs),
                Token.Identity(op1),
                Token.Identity(chs),
                Token.Identity(op2),
                Token.Identity(rhs),
            });

            // true && false && true
            // && true false && true
            // && true && false true

            Assert.AreEqual(
                new[] {
                    Term.Constant(result) },
                actual);
        }

    }
}
