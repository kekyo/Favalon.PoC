using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class TransposeTest
    {
        [Test]
        public void TransposeNonTransposableTerm()
        {
            // abc def ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // (abc def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorPartialTerm1()
        {
            // abc +
            var term =
                Term.Apply(
                    Term.Identity("abc"),
                    Term.Operator("+"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // + abc
            var expected =
                Term.Apply(
                    Term.Operator("+"),
                    Term.Identity("abc"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorPartialTerm2()
        {
            // abc ->
            var term =
                Term.Apply(
                    Term.Identity("abc"),
                    Term.Operator("->"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // -> abc
            var expected =
                Term.Apply(
                    Term.Operator("->"),
                    Term.Identity("abc"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorTerm1()
        {
            // abc + def
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("+")),
                    Term.Identity("def"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // + abc def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("+"),
                        Term.Identity("abc")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorTerm2()
        {
            // abc -> def
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("->")),
                    Term.Identity("def"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // -> abc def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("abc")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorPartialTermWithTrailingApply1()
        {
            // abc def +
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Operator("+"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc + def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("+")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInfixOperatorPartialTermWithTrailingApply2()
        {
            // abc def ->
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Operator("->"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc -> def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("->")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInfixOperatorTermWithApply()
        {
            // abc + def ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Operator("+")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // + abc def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Operator("+"),
                            Term.Identity("abc")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeInfixRightAssociativeOperatorTermWithApply()
        {
            // abc -> def ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Operator("->")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // -> abc (def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapTwoInfixOperatorTermTrailingApplyTerm()
        {
            // abc + (def +)
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("+")),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Operator("+")));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // + abc (+ def)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("+"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Operator("+"),
                        Term.Identity("def")));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeTwoInfixRightAssociativeOperatorTermTrailingApplyTerm()
        {
            // abc -> (def ->)
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Operator("->")),
                    Term.Apply(
                        Term.Identity("def"),
                        Term.Operator("->")));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

         // -> abc (-> def)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("def")));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInnerInfixOperatorTerm()
        {
            // abc (+ def) ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInnerInfixAndTransposeOperatorTerm()
        {
            // abc (-> def) ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc (-> def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapInnerTrailingInfixOperatorTerm()
        {
            // abc (def +) ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("def"),
                            Term.Operator("+"))),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapInnerTrailingInfixAndTransposeOperatorTerm()
        {
            // abc (def ->) ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Identity("def"),
                            Term.Operator("->"))),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc (-> def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapDoubleInfixOperatorTermsAndApply()
        {
            // abc (def +) - ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Apply(
                                Term.Identity("def"),
                                Term.Operator("+"))),
                        Term.Operator("-")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc - (+ def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Operator("-")),
                        Term.Apply(
                            Term.Operator("+"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeDoubleInfixOperatorTermsAndApply()
        {
            // abc (def ->) -> ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Apply(
                                Term.Identity("def"),
                                Term.Operator("->"))),
                        Term.Operator("->")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // abc -> (-> def) ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Operator("->")),
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def"))),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        ////////////////////////////////////////////

        [Test]
        public void SwapTwoInfixOperatorTerm()
        {
            // abc + def + ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("abc"),
                                Term.Operator("+")),
                            Term.Identity("def")),
                        Term.Operator("+")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // + abc + def ghi
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Operator("+"),
                                Term.Identity("abc")),
                            Term.Operator("+")),
                        Term.Identity("def")),
                    Term.Identity("ghi"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SwapAndTransposeTwoInfixRightAssociativeOperatorTerm()
        {
            // abc -> def -> ghi
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Apply(
                                Term.Identity("abc"),
                                Term.Operator("->")),
                            Term.Identity("def")),
                        Term.Operator("->")),
                    Term.Identity("ghi"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // -> abc (-> def ghi)
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("abc")),
                    Term.Apply(
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def")),
                        Term.Identity("ghi")));

            Assert.AreEqual(expected, actual);
        }
    }
}
