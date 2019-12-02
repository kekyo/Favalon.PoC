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
                Term.Constant(123);

            var context = new Context();
            var actual = term.Infer(context);

            Assert.AreEqual(Term.Identity(typeof(int)), actual.HigherOrder);
        }
    }
}
