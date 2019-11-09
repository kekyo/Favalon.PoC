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

        [Test]
        public void TransposeOperatorWithPartialTerm()
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

        [Test]
        public void TransposeOperatorWithSimpleTerm()
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

            // (-> abc) def
            var expected =
                Term.Apply(
                    Term.Apply(
                        Term.Operator("->"),
                        Term.Identity("abc")),
                    Term.Identity("def"));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TransposeOperatorWithTrailingApplyTerm()
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

            // (-> abc) (def ghi)
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

        [Test]
        public void TransposeOperatorWithTrailingApplyOperator()
        {
            // abc -> def ->
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("abc"),
                            Term.Operator("->")),
                        Term.Identity("def")),
                    Term.Operator("->"));

            var environment = Environment.Create();
            var actual = environment.Transpose(term);

            // -> (abc (-> def))
            var expected =
                Term.Apply(
                    Term.Operator("->"),
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Apply(
                            Term.Operator("->"),
                            Term.Identity("def"))));

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TransposeOperatorWithTrailingApplyOperatorSequence()
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
