using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class BooleanAlgebraTest
    {
        [Test]
        public void BooleanTrue()
        {
            var term = new BooleanTerm(true);

            var actual = term.Reduce();

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }

        [Test]
        public void BooleanFalse()
        {
            var term = new BooleanTerm(false);

            var actual = term.Reduce();

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void BooleanAnd(bool result, bool lhs, bool rhs)
        {
            var term =
                new ApplyTerm(
                    new AndTerm(new BooleanTerm(lhs)),
                    new BooleanTerm(rhs));

            var actual = term.Reduce();

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, false, false, true)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, false, false)]
        public void BooleanAndAnd(bool result, bool lhs, bool chs, bool rhs)
        {
            var term =
                new ApplyTerm(
                    new AndTerm(
                        new ApplyTerm(
                            new AndTerm(new BooleanTerm(lhs)),
                            new BooleanTerm(chs))),
                    new BooleanTerm(rhs));

            var actual = term.Reduce();

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        public void LambdaCallAndFixedResult(bool result)
        {
            var term =
                new ApplyTerm(
                    new LambdaTerm(
                        "a",
                        new BooleanTerm(result)),
                new BooleanTerm(false));

            var actual = term.Reduce();

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
