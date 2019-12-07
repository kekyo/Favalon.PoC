using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    class AlgebricDataTest
    {
        //[Test]
        public void DeclareOption()
        {
            // let Option = type a -> match (Some a -> Option a) (None Option a)
            var term =
                Term.Type(
                    Term.Sum(
                        Term.Pair(
                            Term.Identity("Some"),
                            Term.Lambda(Term.Identity("a"), Term.Identity("Some", Term.Identity("Option")))),
                        Term.Identity("None")));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            //var expected =
            //    Term.DiscriminatedUnion(
            //        Term.Identity("Left"),
            //        Term.Identity("Right"));

            //Assert.AreEqual(
            //    expected, actual);
        }

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

            //var expected =
            //    Term.DiscriminatedUnion(
            //        Term.Identity("Left"),
            //        Term.Identity("Right"));

            //Assert.AreEqual(
            //    expected, actual);
        }
    }
}
