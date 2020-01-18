using Favalon.Terms.Logical;
using NUnit.Framework;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

namespace Favalon
{
    [TestFixture]
    class LambdaTest
    {
        //[TestCase(true)]
        //[TestCase(false)]
        //public void LambdaArrowOperatorCallAndFixedResult(bool result)
        //{
        //    var term =
        //        Apply(
        //            Apply(
        //                Apply(
        //                    LambdaOperatorTerm.Instance,
        //                    Identity("a")),
        //                Constant(result)),
        //            Constant(false));

        //    var environment = Environment.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        //}

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallAndFixedResult(bool result)
        {
            var term =
                Apply(
                    Lambda(
                        "a",
                        Constant(result)),
                Constant(false));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallPassingArgument(bool result)
        {
            var term =
                Apply(
                    Lambda(
                        "a",
                        Identity("a")),
                Constant(result));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void NestedLambdaCallPassingArgument(bool result, bool lhs, bool rhs)
        {
            var term =
                Apply(
                    Apply(
                        // a -> b -> a && b
                        Lambda(
                            "a",
                            Lambda(
                                "b",
                                AndAlso(
                                    Identity("a"),
                                    Identity("b")))),
                        Constant(lhs)),
                    Constant(rhs));

            var a = term.DebuggerDisplay;

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
