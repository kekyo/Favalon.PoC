using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    class AlgebricDataTest
    {
        //[Test]
        public void DeclareEither()
        {
            var term =
                Term.Type(
                    Term.Sum(
                        Term.Identity("Left"),
                        Term.Identity("Right")));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);


        }
    }
}
