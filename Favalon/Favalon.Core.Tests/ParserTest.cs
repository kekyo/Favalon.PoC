using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        private static Term[] Parse(Token[] tokens) =>
            Environment.Create().Parse(tokens).ToArray();

        [Test]
        public void EnumerableIdentityToken()
        {
            var actual = Parse(
                new[]
                {
                    Token.Identity("abc"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Identity("abc") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokens()
        {
            var actual = Parse(
                new[]
                {
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("def")),
                        Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithBeforeBracketTokens()
        {
            // (abc def) ghi
            var actual = Parse(
                new Token[]
                {
                    Token.Open('('),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Close(')'),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("def")),
                        Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithAfterBracketTokens()
        {
            // abc (def ghi)
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Open('('),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                    Token.Close(')'),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("def"),
                            Term.Identity("ghi"))) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithAllBracketTokens()
        {
            // (abc def ghi)
            var actual = Parse(
                new Token[]
                {
                    Token.Open('('),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Identity("ghi"),
                    Token.Close(')'),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("def")),
                        Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithBracketToken()
        {
            // abc (def) ghi
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Open('('),
                    Token.Identity("def"),
                    Token.Close(')'),
                    Token.Identity("ghi"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("def")),
                        Term.Identity("ghi")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithNestedBeforeBracketsTokens()
        {
            // ((abc def) ghi) jkl
            var actual = Parse(
                new Token[]
                {
                    Token.Open('('),
                    Token.Open('('),
                    Token.Identity("abc"),
                    Token.Identity("def"),
                    Token.Close(')'),
                    Token.Identity("ghi"),
                    Token.Close(')'),
                    Token.Identity("jkl"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("abc"),
                                Term.Identity("def")),
                            Term.Identity("ghi")),
                        Term.Identity("jkl")) },
                actual);
        }

        [Test]
        public void EnumerableIdentityWithNestedAfterBracketsTokens()
        {
            // abc (def (ghi jkl))
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Open('('),
                    Token.Identity("def"),
                    Token.Open('('),
                    Token.Identity("ghi"),
                    Token.Identity("jkl"),
                    Token.Close(')'),
                    Token.Close(')'),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("def"),
                            Term.Apply(
                                Term.Identity("ghi"),
                                Term.Identity("jkl")))) },
                actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void EnumerableNumericToken()
        {
            var actual = Parse(
                new[]
                {
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Constant(123) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithSign(bool plus)
        {
            // -123    // minus sign
            var actual = Parse(
                new Token[]
                {
                    Token.NumericalSign(plus ? '+' : '-'),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Constant(plus ? 123 : -123) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithOperator(bool plus)
        {
            // - 123    // unary op
            var actual = Parse(
                new Token[]
                {
                    Token.Identity(plus ? "+" : "-"),
                    Token.WhiteSpace(),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Constant(123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenCloseSignAfterIdentity(bool plus)
        {
            // abc -123    // minus sign
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.NumericalSign(plus ? '+' : '-'),
                    Token.Numeric("123"),
                });

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Constant(plus ? 123 : -123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithOperatorAfterIdentity1(bool plus)
        {
            // abc-123     // binary op
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.NumericalSign(plus ? '+' : '-'),
                    Token.Numeric("123"),
                });

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Constant(123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithOperatorAfterIdentity2(bool plus)
        {
            // abc- 123    // binary op
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.Identity(plus ? "+" : "-"),
                    Token.WhiteSpace(),
                    Token.Numeric("123"),
                });

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Constant(123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithOperatorAfterIdentity3(bool plus)
        {
            // abc - 123   // binary op
            var actual = Parse(
                new Token[]
                {
                    Token.Identity("abc"),
                    Token.WhiteSpace(),
                    Token.Identity(plus ? "+" : "-"),
                    Token.WhiteSpace(),
                    Token.Numeric("123"),
                });

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Constant(123)) },
                actual);
        }
    }
}
