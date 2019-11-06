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

            // abc def ghi
            Assert.AreEqual(
                Term.Apply(
                    Term.Apply(
                        Term.Identity("abc"),
                        Term.Identity("def")),
                    Term.Identity("ghi")),
                actual);
        }

        [Test]
        public void TransposeOperatorWithSimpleTerm()
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
            Assert.AreEqual(
                Term.Apply(
                    Term.Apply(
                        Term.Operator("+"),
                        Term.Identity("abc")),
                    Term.Identity("def")),
                actual);
        }
    }
}
