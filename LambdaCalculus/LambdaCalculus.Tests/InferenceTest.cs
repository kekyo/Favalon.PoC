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

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void LambdaConstantBodyTest()
        {
            var term =
                Term.Lambda(
                    "a",
                    Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        //[Test]
        //public void LambdaVariableBodyTest()
        //{
        //    var term =
        //        Term.Lambda(
        //            "a",
        //            Term.Identity("a"));

        //    var environment = Environment.Create();
        //    var actual = environment.Infer(term);

        //    Assert.AreEqual(0, actual.HigherOrder is PlaceholderTerm placeholder ? placeholder.Index : -1);
        //    Assert.AreEqual(Term.Unspecified(), actual.HigherOrder.HigherOrder);
        //}

        [Test]
        public void BooleanAppliedLambdaVariableBodyTest()
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Identity("a")),
                    Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }
    }
}
