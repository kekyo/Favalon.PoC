using Favalon.Terms.Logical;
using NUnit.Framework;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

namespace Favalon
{
    [TestFixture]
    class BooleanTest
    {
        [Test]
        public void TrueTerm()
        {
            var term = TermFactory.True();

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
            Assert.AreEqual(BooleanTerm.Type, actual.HigherOrder);
        }

        [Test]
        public void FalseTerm()
        {
            var term = TermFactory.False();

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
            Assert.AreEqual(BooleanTerm.Type, actual.HigherOrder);
        }

        [Test]
        public void ClrTrueTerm()
        {
            var term = TermFactory.True();

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
            Assert.AreEqual(ClrType<bool>(), actual.HigherOrder);
        }

        [Test]
        public void ClrFalseTerm()
        {
            var term = TermFactory.False();

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
            Assert.AreEqual(ClrType<bool>(), actual.HigherOrder);
        }

        /////////////////////////////////////////////////////////

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void NotTerm(bool result, bool value)
        {
            var term =
                Not(
                    BooleanTerm.From(value));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        //[TestCase(false, true)]
        //[TestCase(true, false)]
        //public void NotOperatorTerm(bool result, bool value)
        //{
        //    var term =
        //        Apply(
        //            NotOperatorTerm.Instance,
        //            BooleanTerm.From(value));

        //    var environment = EnvironmentFactory.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        //}

        /////////////////////////////////////////////////////////

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void AndAlsoTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                AndAlso(
                    BooleanTerm.From(lhs),
                    BooleanTerm.From(rhs));

            var environment = EnvironmentFactory.Create();
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
                AndAlso(
                    AndAlso(
                        BooleanTerm.From(lhs),
                        BooleanTerm.From(chs)),
                    BooleanTerm.From(rhs));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        //[TestCase(true, true, true)]
        //[TestCase(false, false, true)]
        //[TestCase(false, true, false)]
        //[TestCase(false, false, false)]
        //public void AndAlsoOperatorTerm(bool result, bool lhs, bool rhs)
        //{
        //    var term =
        //        Apply(
        //            Apply(
        //                Terms.Operators.AndAlsoOperatorTerm.Instance,
        //                BooleanTerm.From(lhs)),
        //            BooleanTerm.From(rhs));

        //    var environment = Environment.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        //}

        /////////////////////////////////////////////////////////

        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        [TestCase(false, false, false)]
        public void OrElseTerm(bool result, bool lhs, bool rhs)
        {
            var term =
                OrElse(
                    BooleanTerm.From(lhs),
                    BooleanTerm.From(rhs));

            var environment = EnvironmentFactory.Create();
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
                OrElse(
                    OrElse(
                        BooleanTerm.From(lhs),
                        BooleanTerm.From(chs)),
                    BooleanTerm.From(rhs));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        //[TestCase(true, true, true)]
        //[TestCase(true, false, true)]
        //[TestCase(true, true, false)]
        //[TestCase(false, false, false)]
        //public void OrElseOperatorTerm(bool result, bool lhs, bool rhs)
        //{
        //    var term =
        //        Apply(
        //            Apply(
        //                Terms.Operators.OrElseOperatorTerm.Instance,
        //                BooleanTerm.From(lhs)),
        //            BooleanTerm.From(rhs));

        //    var environment = EnvironmentFactory.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        //}

        /////////////////////////////////////////////////////////

        //[TestCase(true, 123)]
        //[TestCase(false, 100)]
        //public void EqualTerm(bool condition, int value)
        //{
        //    var term =
        //        Equal(
        //            BooleanTerm.From(123),
        //            BooleanTerm.From(value));

        //    var environment = EnvironmentFactory.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(condition, ((BooleanTerm)actual).Value);
        //}

        //[TestCase(true, 123)]
        //[TestCase(false, 100)]
        //public void EqualOperatorTerm(bool condition, int value)
        //{
        //    var term =
        //        Apply(
        //            Apply(
        //                Terms.Operators.EqualOperatorTerm.Instance,
        //                BooleanTerm.From(123)),
        //            BooleanTerm.From(value));

        //    var environment = EnvironmentFactory.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(condition, ((BooleanTerm)actual).Value);
        //}
    }
}
