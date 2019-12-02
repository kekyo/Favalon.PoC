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
    }
}
