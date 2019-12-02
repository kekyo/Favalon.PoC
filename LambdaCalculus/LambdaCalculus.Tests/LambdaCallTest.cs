using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class LambdaCallTest
    {
        [TestCase(true)]
        [TestCase(false)]
        public void BasisLambdaCallAndFixedResult(bool result)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("->"),
                            Term.Identity("a")),
                        Term.Constant(result)),
                    Term.Constant(false));

            var context = new Context();
            context.AddBoundTerm("->", LambdaArrowTerm.Instance);

            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallAndFixedResult(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Constant(result)),
                Term.Constant(false));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallPassingArgument(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Identity("a")),
                Term.Constant(result));

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
                Term.Apply(
                    Term.Apply(
                        // a -> b -> a && b
                        Term.Lambda(
                            "a",
                            Term.Lambda(
                                "b",
                                Term.Apply(
                                    Term.And(
                                        Term.Identity("a")),
                                    Term.Identity("b")))),
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
