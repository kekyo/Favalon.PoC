using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class AlgebricDataTest
    {
        //[Test]
        public void Either()
        {
            var term =
                Term.Sum(
                    Term.Identity("Left"),
                    Term.Identity("Right"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            //Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }
    }
}
