using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class InferenceTest
    {
        [Test]
        public void ConstantTest()
        {
            var term =
                Term.Constant(false);

            var context = new Context();
            var actual = term.Infer(context);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void LambdaConstantBodyTest()
        {
            var term =
                Term.Lambda(
                    "a",
                    Term.Constant(false));

            var context = new Context();
            var actual = term.Infer(context);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void LambdaVariableBodyTest()
        {
            var term =
                Term.Lambda(
                    "a",
                    Term.Identity("a"));

            var context = new Context();
            var actual = term.Infer(context);

            Assert.AreEqual(Term.Unspecified(), actual.HigherOrder);
        }

        [Test]
        public void BooleanAppliedLambdaVariableBodyTest()
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Identity("a")),
                    Term.Constant(false));

            var context = new Context();
            var actual = term.Infer(context);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }
    }
}
