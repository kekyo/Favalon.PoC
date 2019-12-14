using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    class BindTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void BindInConstant(bool result)
        {
            var term =
                Term.Bind(
                    "a",
                    Term.Constant(result),
                    Term.Not(
                        Term.Identity("a")));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindInAppliedIdentity(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "b",
                        Term.Bind(
                            "a",
                            Term.Identity("b"),
                            Term.Not(
                                Term.Identity("a")))),
                    Term.Constant(result));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindConstantWithSideEffect(bool result)
        {
            var term =
                Term.Bind(
                    "a",
                    Term.Not(
                        Term.Constant(result)));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
            Assert.AreEqual(result, !((BooleanTerm)environment.LookupBoundTerm("a")!).Value);
        }
    }
}
