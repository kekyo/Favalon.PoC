using Favalon.Terms;
using Favalon.Tokens;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class BoundAttributesTest
    {
        private static Term Parse(Token[] tokens)
        {
            var environment = Environment.Create(true);
            environment.AddBoundTerm(
                "+", true, false, BoundTermPrecedences.ArithmericAddition,
                new IdentityTerm("+"));
            environment.AddBoundTerm(
                "-", true, false, BoundTermPrecedences.ArithmericAddition,
                new IdentityTerm("-"));
            environment.AddBoundTerm(
                "*", true, false, BoundTermPrecedences.ArithmericMultiplication,
                new IdentityTerm("*"));
            environment.AddBoundTerm(
                "<<<", true, true, BoundTermPrecedences.Morphism,
                new IdentityTerm("<<<"));
            return environment.Parse(tokens).Single();
        }

        [Test]
        public void TransposeNonTransposableTerm()
        {
            // abc def ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi")
            };

            var actual = Parse(tokens);

            // (abc def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorPartialTerm1()
        {
            // abc +
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("+")
            };

            var actual = Parse(tokens);

            // + abc
            var expected =
                Term.Apply(
                    Term.Identity("+"),
                    Term.Identity("abc"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorPartialTerm2()
        {
            // abc <<<
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("<<<")
            };

            var actual = Parse(tokens);

            // <<< abc
            var expected =
                Term.Apply(
                    Term.Identity("<<<"),
                    Term.Identity("abc"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorTerm1()
        {
            // abc + def
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def")
            };

            var actual = Parse(tokens);

            // + abc def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Identity("abc")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorTerm2()
        {
            // abc <<< def
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("<<<"),
                Token.Identity("def")
            };

            var actual = Parse(tokens);

            // <<< abc def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("<<<"),
                        Term.Identity("abc")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorPartialTermWithTrailingApply1()
        {
            // abc def +
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("+"),
            };

            var actual = Parse(tokens);

            // abc + def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("+")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorPartialTermWithTrailingApply2()
        {
            // abc def <<<
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("<<<"),
            };

            var actual = Parse(tokens);

            // abc <<< def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("<<<")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorTermWithApply()
        {
            // abc + def ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // + abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("+"),
                            Term.Identity("abc")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeInfixRightAssociativeOperatorTermWithApply()
        {
            // abc <<< def ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("<<<"),
                Token.Identity("def"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // <<< abc (def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("<<<"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapTwoInfixOperatorTermTrailingApplyTerm()
        {
            // abc + (def +)
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Close(')')
            };

            var actual = Parse(tokens);

            // + abc (+ def)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Identity("def")));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeTwoInfixRightAssociativeOperatorTermTrailingApplyTerm()
        {
            // abc <<< (def <<<)
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("<<<"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("<<<"),
                Token.Close(')')
            };

            var actual = Parse(tokens);

            // <<< abc (<<< def)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("<<<"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Identity("<<<"),
                        Term.Identity("def")));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInnerInfixOperatorTerm()
        {
            // abc (+ def) ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInnerInfixAndTransposeOperatorTerm()
        {
            // abc (<<< def) ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("<<<"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc (<<< def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("<<<"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapInnerTrailingInfixOperatorTerm()
        {
            // abc (def +) ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInnerTrailingInfixAndTransposeOperatorTerm()
        {
            // abc (def <<<) ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("<<<"),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc (<<< def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("<<<"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapDoubleInfixOperatorTermsAndApply()
        {
            // abc (def +) - ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Close(')'),
                Token.Identity("-"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc - (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("-")),
                        Term.Apply(
                            Term.Identity("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeDoubleInfixOperatorTermsAndApply()
        {
            // abc (def <<<) <<< ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("<<<"),
                Token.Close(')'),
                Token.Identity("<<<"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc <<< (<<< def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Identity("<<<")),
                        Term.Apply(
                            Term.Identity("<<<"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void SwapTwoInfixOperatorTerm()
        {
            // abc + def + ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // + abc + def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("abc")),
                            Term.Identity("+")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeTwoInfixRightAssociativeOperatorTerm()
        {
            // abc <<< def <<< ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("<<<"),
                Token.Identity("def"),
                Token.Identity("<<<"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // <<< abc (<<< def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("<<<"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("<<<"),
                            Term.Identity("def")),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void ForwardOrderedTerms()
        {
            // abc * def + ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("*"),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // * abc + def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("abc")),
                            Term.Identity("+")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BackwardOrderedTerms()
        {
            // abc + def * ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Identity("*"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // + abc (* def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("*"),
                            Term.Identity("def")),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }
    }
}
