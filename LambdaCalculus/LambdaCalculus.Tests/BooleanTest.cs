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
            var term = True();

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }

        [Test]
        public void FalseTerm()
        {
            var term = False();

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
                Not(
                    Constant(value));

            var environment = Environment.Create();
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
        //            Constant(value));

        //    var environment = Environment.Create();
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
                    Constant(lhs),
                    Constant(rhs));

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
                AndAlso(
                    AndAlso(
                        Constant(lhs),
                        Constant(chs)),
                    Constant(rhs));

            var environment = Environment.Create();
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
        //                Constant(lhs)),
        //            Constant(rhs));

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
                    Constant(lhs),
                    Constant(rhs));

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
                OrElse(
                    OrElse(
                        Constant(lhs),
                        Constant(chs)),
                    Constant(rhs));

            var environment = Environment.Create();
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
        //                Constant(lhs)),
        //            Constant(rhs));

        //    var environment = Environment.Create();
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
        //            Constant(123),
        //            Constant(value));

        //    var environment = Environment.Create();
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
        //                Constant(123)),
        //            Constant(value));

        //    var environment = Environment.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(condition, ((BooleanTerm)actual).Value);
        //}
    }
}
