using Favalon.Terms;
//using Favalon.Terms.Operators;
using NUnit.Framework;

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
        //        Term.Apply(
        //            Term.Apply(
        //                Term.Apply(
        //                    LambdaOperatorTerm.Instance,
        //                    Term.Identity("a")),
        //                Term.Constant(result)),
        //            Term.Constant(false));

        //    var environment = Environment.Create();
        //    var actual = environment.Reduce(term);

        //    Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        //}

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallAndFixedResult(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Constant(result)),
                Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LambdaCallPassingArgument(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Identity("a")),
                Term.Constant(result));

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
                Term.Apply(
                    Term.Apply(
                        // a -> b -> a && b
                        Term.Lambda(
                            "a",
                            Term.Lambda(
                                "b",
                                Term.AndAlso(
                                    Term.Identity("a"),
                                    Term.Identity("b")))),
                        Term.Constant(lhs)),
                    Term.Constant(rhs));

            var a = term.DebuggerDisplay;

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
