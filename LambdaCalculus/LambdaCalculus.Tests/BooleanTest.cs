using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
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

        /////////////////////////////////////////////////////////

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void NotTerm(bool result, bool value)
        {
            var term =
                Term.Not(
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void NotOperatorTerm(bool result, bool value)
        {
            var term =
                Term.Apply(
                    Terms.Operators.NotOperatorTerm.Instance,
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        /////////////////////////////////////////////////////////

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AndAlsoTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.AndAlso(
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
        public void DoubleAndAlsoTerm(bool result, bool lhs, bool chs, bool rhs)
        {
            var term =
                Term.AndAlso(
                    Term.AndAlso(
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
        public void AndAlsoOperatorTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Terms.Operators.AndAlsoOperatorTerm.Instance,
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        /////////////////////////////////////////////////////////

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        [TestCase(false, false, false)]
        public void OrElseTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.OrElse(
                    Term.Constant(lhs),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true, true)]
        [TestCase(true, false, true, true)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        [TestCase(true, false, false, true)]
        [TestCase(true, true, false, false)]
        [TestCase(false, false, false, false)]
        public void DoubleOrElseTerm(bool result, bool lhs, bool chs, bool rhs)
        {
            var term =
                Term.OrElse(
                    Term.OrElse(
                        Term.Constant(lhs),
                        Term.Constant(chs)),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        [TestCase(false, false, false)]
        public void OrElseOperatorTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Terms.Operators.OrElseOperatorTerm.Instance,
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        /////////////////////////////////////////////////////////

        [TestCase(true, 123)]
        [TestCase(false, 100)]
        public void EqualTerm(bool condition, int value)
        {
            var term =
                Term.Equal(
                    Term.Constant(123),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(condition, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, 123)]
        [TestCase(false, 100)]
        public void EqualOperatorTerm(bool condition, int value)
        {
            var term =
                Term.Apply(
                    Term.Apply(
                        Terms.Operators.EqualOperatorTerm.Instance,
                        Term.Constant(123)),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(condition, ((BooleanTerm)actual).Value);
        }
    }
}
