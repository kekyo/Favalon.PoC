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

            Assert.AreEqual(Term.Identity(typeof(bool)), actual.HigherOrder);
        }
    }
}
