using Favalon.Terms.Logical;
using NUnit.Framework;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

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
                Bind(
                    "a",
                    Constant(result),
                    Not(
                        Identity("a")));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindInAppliedIdentity(bool result)
        {
            var term =
                Apply(
                    Lambda(
                        "b",
                        Bind(
                            "a",
                            Identity("b"),
                            Not(
                                Identity("a")))),
                    Constant(result));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindConstantWithSideEffect(bool result)
        {
            var term =
                Bind(
                    "a",
                    Not(
                        Constant(result)));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, !((BooleanTerm)actual).Value);
            Assert.AreEqual(result, !((BooleanTerm)environment.LookupBoundTerm("a")!).Value);
        }
    }
}
