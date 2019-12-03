using NUnit.Framework;

namespace LambdaCalculus
{
    [TestFixture]
    class BooleanTest
    {
        [Test]
        public void TrueTerm()
        {
            var term = Term.True();

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }

        [Test]
        public void FalseTerm()
        {
            var term = Term.False();

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AndTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.And(
                    Term.Constant(lhs),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true, true)]
        [TestCase(false, false, true, true)]
        [TestCase(false, true, false, true)]
        [TestCase(false, true, true, false)]
        [TestCase(false, false, false, true)]
        [TestCase(false, true, false, false)]
        [TestCase(false, false, false, false)]
        public void DoubleAndTerm(bool result, bool lhs, bool chs, bool rhs)
        {
            var term =
                Term.And(
                    Term.And(
                        Term.Constant(lhs),
                        Term.Constant(chs)),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AndOperatorTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        LambdaCalculus.AndOperatorTerm.Instance,
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, "a")]
        [TestCase(false, "b")]
        public void IfTerm(bool condition, string result)
        {
            var term =
                Term.If(
                    Term.Constant(condition),
                    Term.Identity("a"),
                    Term.Identity("b"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((IdentityTerm)actual).Identity);
        }

        [TestCase(true, "a")]
        [TestCase(false, "b")]
        public void IfOperatorTerm(bool condition, string result)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            LambdaCalculus.IfOperatorTerm.Instance,
                            Term.Constant(condition)),
                        Term.Identity("a")),
                    Term.Identity("b"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((IdentityTerm)actual).Identity);
        }
    }
}
