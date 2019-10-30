using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void EnumerableIdentityToken()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Identity("abc"),
                });

            Assert.AreEqual(
                new[] { Term.Identity("abc") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokens()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithBeforeBracketTokens()
        {
            // (abc def) ghi
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Begin(),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.End(),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithAfterBracketTokens()
        {
            // abc (def ghi)
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Begin(),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                    Token.End(),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Identity("abc"), Term.Apply(Term.Identity("def"), Term.Identity("ghi"))) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithAllBracketTokens()
        {
            // (abc def ghi)
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Begin(),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                    Token.End(),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithBracketToken()
        {
            // abc (def) ghi
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Begin(),
                    Token.Identity("def"),
                    Token.End(),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithNestedBeforeBracketsTokens()
        {
            // ((abc def) ghi) jkl
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Begin(),
                    Token.Begin(),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.End(),
                    Token.Identity("ghi"),
                    Token.End(),
                    Token.Identity("jkl"),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("def")), Term.Identity("ghi")), Term.Identity("jkl")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithNestedAfterBracketsTokens()
        {
            // abc (def (ghi jkl))
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Begin(),
                    Token.Identity("def"),
                    Token.Begin(),
                    Token.Identity("ghi"),
                    Token.Identity("jkl"),
                    Token.End(),
                    Token.End(),
                });

            Assert.AreEqual(
                new[] { Term.Apply(Term.Identity("abc"), Term.Apply(Term.Identity("def"), Term.Apply(Term.Identity("ghi"), Term.Identity("jkl")))) },
                actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void EnumerableNumericToken()
        {
            var actual = Parser.EnumerableTerms(
                new[]
                {
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] { Term.Constant(123) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithPlusSign()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("+"), Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] { Term.Constant(123) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithMinusSign()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("-"), Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] { Term.Constant(-123) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithPlusOperator()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("+"), Token.WhiteSpace(), Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] { Term.Constant(123) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithMinusOperator()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("-"), Token.WhiteSpace(), Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] { Term.Constant(-123) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenClosePlusSignAfterIdentity()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("+"),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new Term[] { Term.Apply(Term.Identity("abc"), Term.Constant(123)) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenCloseMinusSignAfterIdentity()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity("-"),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new Term[] { Term.Apply(Term.Identity("abc"), Term.Constant(-123)) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithPlusOperatorAfterIdentity()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Identity("+"),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new Term[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("+")), Term.Constant(123)) },
                actual);
        }

        [Test]
        public void EnumerableNumericTokenWithMinusOperatorAfterIdentity()
        {
            var actual = Parser.EnumerableTerms(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Identity("-"),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new Term[] { Term.Apply(Term.Apply(Term.Identity("abc"), Term.Identity("-")), Term.Constant(123)) },
                actual);
        }
    }
}
