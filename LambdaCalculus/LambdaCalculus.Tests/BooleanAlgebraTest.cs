using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class BooleanAlgebraTest
    {
        [Test]
        public void BooleanTrue()
        {
            var term = Term.True();

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }

        [Test]
        public void BooleanFalse()
        {
            var term = Term.False();

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void BooleanAnd(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.Apply(
                    Term.And(
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var context = new Context();
            var actual = term.Reduce(context);

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
                Term.Apply(
                    Term.And(
                        Term.Apply(
                            Term.And(
                                Term.Constant(lhs)),
                            Term.Constant(chs))),
                    Term.Constant(rhs));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, "a")]
        [TestCase(false, "b")]
        public void If(bool condition, string result)
        {
            var term =
                Term.If(
                    Term.Constant(condition),
                    Term.Identity("a"),
                    Term.Identity("b"));

            var context = new Context();
            var actual = term.Reduce(context);

            Assert.AreEqual(result, ((IdentityTerm)actual).Identity);
        }
    }
}
