using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class LambdaCallTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallAndFixedResult(bool result)
        {
            var term =
                new ApplyTerm(
                    new LambdaTerm(
                        "a",
                        new BooleanTerm(result)),
                new BooleanTerm(false));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallPassingArgument(bool result)
        {
            var term =
                new ApplyTerm(
                    new LambdaTerm(
                        "a",
                        new IdentityTerm("a")),
                new BooleanTerm(result));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void NestedLambdaCallPassingArgument(bool result, bool lhs, bool rhs)
        {
            var term =
                new ApplyTerm(
                    new ApplyTerm(
                        // a -> b -> a && b
                        new LambdaTerm(
                            "a",
                            new LambdaTerm(
                                "b",
                                new AndTerm(new IdentityTerm("a"), new IdentityTerm("b")))),
                        new BooleanTerm(lhs)),
                    new BooleanTerm(rhs));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
