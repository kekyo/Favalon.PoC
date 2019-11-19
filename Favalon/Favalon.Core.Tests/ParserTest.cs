using Favalon.Tokens;
using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        private static Term[] Parse(params Token[] tokens) =>
            Environment.Create().Parse(tokens).ToArray();

        [Test]
        public void EnumerableIdentityToken()
        {
            var actual = Parse(
                Token.Identity("abc"));

            Assert.AreEqual(
                new[] {
                    Term.Identity("abc") },
                actual);
        }

        [Test]
        public void EnumerableIdentityTokens()
        {
            var actual = Parse(
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi"));

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
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"));

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
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("ghi"),
                Token.Close(')'));

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
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi"),
                Token.Close(')'));

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
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"));

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
                Token.Open('('),
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Identity("jkl"));

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
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Open('('),
                Token.Identity("ghi"),
                Token.Identity("jkl"),
                Token.Close(')'),
                Token.Close(')'));

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
                Token.Numeric("123"));

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
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"));

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
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Numeric("123"));

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Constant(123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithBracketedSign(bool plus)
        {
            // (-123)    // minus sign
            var actual = Parse(
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"),
                Token.Close(')'));

            Assert.AreEqual(
                new[] {
                    Term.Constant(plus ? 123 : -123) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenWithBracketedOperator(bool plus)
        {
            // (- 123)    // unary op
            var actual = Parse(
                Token.Open('('),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Numeric("123"),
                Token.Close(')'));

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
                Token.Identity("abc"),
                Token.WhiteSpace(),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Constant(plus ? 123 : -123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenCloseBracketedSignAfterIdentity(bool plus)
        {
            // abc (-123)    // minus sign
            var actual = Parse(
                Token.Identity("abc"),
                Token.WhiteSpace(),
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"),
                Token.Close(')'));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Constant(plus ? 123 : -123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenCloseNonSpacedBracketedSignAfterIdentity(bool plus)
        {
            // abc(-123)    // minus sign
            var actual = Parse(
                Token.Identity("abc"),
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"),
                Token.Close(')'));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Constant(plus ? 123 : -123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenCloseSignAfterBracketedIdentity(bool plus)
        {
            // (abc) -123    // minus sign
            var actual = Parse(
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
                Token.WhiteSpace(),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Constant(plus ? 123 : -123)) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableNumericTokenCloseSignAfterNonSpacedBracketedIdentity(bool plus)
        {
            // (abc)-123    // binary op
            var actual = Parse(
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"));

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
        public void EnumerableNumericTokenWithOperatorAfterIdentity1(bool plus)
        {
            // abc-123     // binary op
            var actual = Parse(
                Token.Identity("abc"),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Numeric("123"));

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
                Token.Identity("abc"),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Numeric("123"));

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
                Token.Identity("abc"),
                Token.WhiteSpace(),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Numeric("123"));

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
        public void EnumerableIdentityTokenWithSign(bool plus)
        {
            // -abc    // unary op
            var actual = Parse(
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("abc"));

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Identity("abc")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithOperator(bool plus)
        {
            // - abc    // unary op
            var actual = Parse(
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Identity("abc"));

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Identity("abc")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithBracketedSign(bool plus)
        {
            // (-abc)    // unary op
            var actual = Parse(
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("abc"),
                Token.Close(')'));

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Identity("abc")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithBracketedOperator(bool plus)
        {
            // (- abc)    // unary op
            var actual = Parse(
                Token.Open('('),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Identity("abc"),
                Token.Close(')'));

            Assert.AreEqual(
                new[] {
                    Term.Apply(
                        Term.Identity(plus ? "+" : "-"),
                        Term.Identity("abc")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenCloseSignAfterIdentity(bool plus)
        {
            // abc -def    // binary op
            var actual = Parse(
                Token.Identity("abc"),
                Token.WhiteSpace(),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithOperatorAfterIdentity(bool plus)
        {
            // abc-def     // binary op
            var actual = Parse(
                Token.Identity("abc"),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"));

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenCloseBracketedSignAfterIdentity(bool plus)
        {
            // abc (-def)    // unary op
            var actual = Parse(
                Token.Identity("abc"),
                Token.WhiteSpace(),
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"),
                Token.Close(')'));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity(plus ? "+" : "-"),
                            Term.Identity("def"))) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithBracketedOperatorAfterIdentity(bool plus)
        {
            // abc(-def)     // unary op
            var actual = Parse(
                Token.Identity("abc"),
                Token.Open('('),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"),
                Token.Close(')'));

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity(plus ? "+" : "-"),
                            Term.Identity("def"))) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenCloseSignAfterBracketedIdentity(bool plus)
        {
            // (abc) -def    // binary op
            var actual = Parse(
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
                Token.WhiteSpace(),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"));

            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithOperatorAfterBracketedIdentity(bool plus)
        {
            // (abc)-def     // binary op
            var actual = Parse(
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
                plus ? Token.PlusSign() : Token.MinusSign(),
                Token.Identity("def"));

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithOperatorAfterIdentity2(bool plus)
        {
            // abc- def    // binary op
            var actual = Parse(
                Token.Identity("abc"),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Identity("def"));

            // abc - def
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void EnumerableIdentityTokenWithOperatorAfterIdentity3(bool plus)
        {
            // abc - def   // binary op
            var actual = Parse(
                Token.Identity("abc"),
                Token.WhiteSpace(),
                Token.Identity(plus ? "+" : "-"),
                Token.WhiteSpace(),
                Token.Identity("def"));

            // abc - 123
            Assert.AreEqual(
                new Term[] {
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity(plus ? "+" : "-")),
                        Term.Identity("def")) },
                actual);
        }
    }
}
