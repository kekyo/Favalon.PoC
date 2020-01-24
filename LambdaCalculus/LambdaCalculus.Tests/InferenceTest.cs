using Favalon.Terms;
using Favalon.Terms.Logical;
using NUnit.Framework;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

namespace Favalon
{
    [TestFixture]
    class InferenceTest
    {
        [Test]
        public void ConstantTest()
        {
            // false
            var term =
                Constant(false);

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            // false:bool
            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void LambdaConstantBodyTest()
        {
            // a -> false
            var term =
                Lambda(
                    "a",
                    Constant(false));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            var lambda = (LambdaTerm)actual;
            var higherOrder = (LambdaTerm)lambda.HigherOrder;

            // a:'0 -> false:bool
            Assert.IsTrue(higherOrder.Parameter is PlaceholderTerm);
            Assert.AreEqual(Constant(typeof(bool)), higherOrder.Body);
        }

        [Test]
        public void LambdaVariableBodyTest()
        {
            // a -> a
            var term =
                Lambda(
                    "a",
                    Identity("a"));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            var lambda = (LambdaTerm)actual;
            var higherOrder = (LambdaTerm)lambda.HigherOrder;

            // a:'0 -> a:'0
            Assert.IsTrue(higherOrder.Parameter is PlaceholderTerm);
            Assert.IsTrue(higherOrder.Body is PlaceholderTerm);

            Assert.AreEqual(higherOrder.Parameter, higherOrder.Body);
        }

        [Test]
        public void BooleanAppliedLambdaVariableBodyTest()
        {
            // (a -> a) false
            var term =
                Apply(
                    Lambda(
                        "a",
                        Identity("a")),
                    Constant(false));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            // (a:bool -> a:bool) false:bool
            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindConstant(bool result)
        {
            var term =
                Bind(
                    "a",
                    Constant(result),
                    Not(
                        Identity("a")));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindAppliedIdentity(bool result)
        {
            var term =
                Apply(
                    Lambda(
                        "b",
                        Bind(
                            "a",
                            Identity("b"),
                            Not(
                                Identity("a")))),
                    Constant(result));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void BooleanAndBodyTest()
        {
            // a && b
            var term =
                AndAlso(
                    Identity("a"),
                    Identity("b"));

            var environment = ClrEnvironmentFactory.Create();
            var actual = (AndAlsoTerm)environment.Reduce(term);

            // (a:bool && b:bool):bool
            Assert.AreEqual(Constant(typeof(bool)), actual.Lhs.HigherOrder);
            Assert.AreEqual(Constant(typeof(bool)), actual.Rhs.HigherOrder);
            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void DoubleLambdaInsideBooleanAndBodyTest()
        {
            // a -> b -> a && b
            var term =
                Lambda(
                    "a",
                    Lambda(
                        "b",
                        AndAlso(
                            Identity("a"),
                            Identity("b"))));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            var lambda = (LambdaTerm)actual;
            var higherOrder = (LambdaTerm)lambda.HigherOrder;

            // a:bool -> (b:bool -> (a:bool && b:bool):bool):bool
            Assert.AreEqual(Constant(typeof(bool)), higherOrder.Parameter);
            Assert.AreEqual(Constant(typeof(bool)), ((LambdaTerm)higherOrder.Body).Parameter);
            Assert.AreEqual(Constant(typeof(bool)), ((LambdaTerm)higherOrder.Body).Body);
        }

        [Test]
        public void PassingLambdaArgument()
        {
            // (a -> b -> a b) (a -> a) false
            var term =
                Apply(
                    Apply(
                        Lambda(
                            "a",
                            Lambda(
                                "b",
                                Apply(
                                    Identity("a"),
                                    Identity("b")))),
                        Lambda(
                            "a",
                            Identity("a"))),
                    Constant(false));

            var environment = ClrEnvironmentFactory.Create();
            var actual = environment.Infer(term);

            // (a:(bool -> bool) -> b:bool -> a:(bool -> bool) b:bool) (a:bool -> a:bool):(bool -> bool) false:bool
            Assert.AreEqual(Constant(typeof(bool)), actual.HigherOrder);
        }
    }
}
