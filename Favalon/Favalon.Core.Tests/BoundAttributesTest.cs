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
                "+", BoundTermNotations.Infix, BoundTermAssociatives.LeftToRight, BoundTermPrecedences.ArithmericAddition,
                new IdentityTerm("+"));
            environment.AddBoundTerm(
                "-", BoundTermNotations.Infix, BoundTermAssociatives.LeftToRight, BoundTermPrecedences.ArithmericAddition,
                new IdentityTerm("-"));
            environment.AddBoundTerm(
                "*", BoundTermNotations.Infix, BoundTermAssociatives.LeftToRight, BoundTermPrecedences.ArithmericMultiplication,
                new IdentityTerm("*"));
            environment.AddBoundTerm(
                "<<<", BoundTermNotations.Infix, BoundTermAssociatives.RightToLeft, BoundTermPrecedences.Morphism,
                new IdentityTerm("<<<"));
            return environment.Parse(tokens).Single();
        }

        //////////////////////////////////////////////

        [Test]
        public void SingleTerm()
        {
            // abc
            var tokens = new[] {
                Token.Identity("abc"),
            };

            var actual = Parse(tokens);

            // abc
            var expected =
                Term.Identity("abc");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MultipleTerm()
        {
            // abc def ghi
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi")
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SingleTermWithBracket()
        {
            // (abc)
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc
            var expected =
                Term.Identity("abc");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AllTermsInsideBracket()
        {
            // (abc def ghi)
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi"),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoubleBrackets1()
        {
            // ((abc def ghi))
            var tokens = new Token[] {
                Token.Open('('),
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoubleBrackets2()
        {
            // abc ((def)) ghi
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Open('('),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TermsInsideMultipleBrackets1()
        {
            // ((abc def) ghi)
            var tokens = new Token[] {
                Token.Open('('),
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TermsInsideMultipleBrackets2()
        {
            // (abc (def ghi))
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc (def ghi)
            var expected =
                Term.Apply(
                    Term.Identity("abc"),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TermsInsideMultipleBrackets3()
        {
            // (abc (def) ghi)
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TermsInsideMultipleBrackets4()
        {
            // ((abc) def) ghi
            var tokens = new Token[] {
                Token.Open('('),
                Token.Open('('),
                Token.Identity("abc"),
                Token.Close(')'),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TermsInsideMultipleBrackets5()
        {
            // abc (def (ghi))
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Open('('),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Close(')'),
            };

            var actual = Parse(tokens);

            // abc (def ghi)
            var expected =
                Term.Apply(
                    Term.Identity("abc"),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Identity("ghi")));

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

            // + (* abc def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("abc")),
                            Term.Identity("def"))),
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

        [Test]
        public void CombineForwardAndBackwardOrderedTerms()
        {
            // abc * def + ghi * jkl
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("*"),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Identity("ghi"),
                Token.Identity("*"),
                Token.Identity("jkl"),
            };

            var actual = Parse(tokens);

            // + (* abc def) (* ghi jkl)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("abc")),
                            Term.Identity("def"))),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("*"),
                            Term.Identity("ghi")),
                        Term.Identity("jkl")));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CombineBackwardAndForwardOrderedTerms()
        {
            // abc + def * ghi + jkl
            var tokens = new[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Identity("*"),
                Token.Identity("ghi"),
                Token.Identity("+"),
                Token.Identity("jkl"),
            };

            var actual = Parse(tokens);

            // + abc + (* def ghi) jkl
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("abc")),
                            Term.Identity("+")),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("def")),
                            Term.Identity("ghi"))),
                    Term.Identity("jkl"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void BackwardOrderedWithBeforeBracketedTerms()
        {
            // (abc * def) + ghi
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("*"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("+"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // + (* abc def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("+"),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("abc")),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ForwardOrderedWithBeforeBracketedTerms()
        {
            // (abc + def) * ghi
            var tokens = new Token[] {
                Token.Open('('),
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Close(')'),
                Token.Identity("*"),
                Token.Identity("ghi"),
            };

            var actual = Parse(tokens);

            // * (+ abc def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("*"),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("abc")),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BackwardOrderedWithAfterBracketedTerms()
        {
            // abc * (def + ghi)
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("*"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Identity("ghi"),
                Token.Close(')')
            };

            var actual = Parse(tokens);

            // * abc (+ def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("*"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("+"),
                            Term.Identity("def")),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ForwardOrderedWithAfterBracketedTerms()
        {
            // abc + (def * ghi)
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("*"),
                Token.Identity("ghi"),
                Token.Close(')')
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

        [Test]
        public void CombineBackwardAndForwardOrderedWithBracketedTerms()
        {
            // abc + (def * ghi) + jkl
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("*"),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Identity("+"),
                Token.Identity("jkl"),
            };

            var actual = Parse(tokens);

            // + abc + (* def ghi) jkl
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("abc")),
                            Term.Identity("+")),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("def")),
                            Term.Identity("ghi"))),
                    Term.Identity("jkl"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CombineForwardAndBackwardOrderedWithBracketedTerms()
        {
            // abc * (def + ghi) * jkl
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("*"),
                Token.Open('('),
                Token.Identity("def"),
                Token.Identity("+"),
                Token.Identity("ghi"),
                Token.Close(')'),
                Token.Identity("*"),
                Token.Identity("jkl"),
            };

            var actual = Parse(tokens);

            // * abc * (+ def ghi) jkl
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("abc")),
                            Term.Identity("*")),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("def")),
                            Term.Identity("ghi"))),
                    Term.Identity("jkl"));

            Assert.AreEqual(expected, actual);
        }

        //////////////////////////////////////////////

        [Test]
        public void ComplexTerms()
        {
            // abc + def * ghi <<< jkl * (mno + pqr) stu
            var tokens = new Token[] {
                Token.Identity("abc"),
                Token.Identity("+"),
                Token.Identity("def"),
                Token.Identity("*"),
                Token.Identity("ghi"),
                Token.Identity("<<<"),
                Token.Identity("jkl"),
                Token.Identity("*"),
                Token.Open('('),
                Token.Identity("mno"),
                Token.Identity("+"),
                Token.Identity("pqr"),
                Token.Close(')'),
                Token.Identity("stu"),
            };

            var actual = Parse(tokens);

            // + abc <<< (* def ghi) (* jkl (+ mno pqr) stu)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("+"),
                                Term.Identity("abc")),
                            Term.Identity("<<<")),
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("def")),
                            Term.Identity("ghi"))),
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("*"),
                                Term.Identity("jkl")),
                            Term.Apply(
                                Term.Apply(
                                    Term.Identity("+"),
                                    Term.Identity("mno")),
                                Term.Identity("pqr"))),
                        Term.Identity("stu")));

            Assert.AreEqual(expected, actual);
        }
    }
}
