using Favalon.Terms;
//using Favalon.Terms.Logical;
//using Favalon.Terms.Operators;
using NUnit.Framework;

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
                Term.Constant(false);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            // false:bool
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void LambdaConstantBodyTest()
        {
            // a -> false
            var term =
                Term.Lambda(
                    "a",
                    Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            var lambda = (LambdaTerm)actual;
            var higherOrder = (LambdaTerm)lambda.HigherOrder;

            // a:'0 -> false:bool
            Assert.IsTrue(higherOrder.Parameter is PlaceholderTerm);
            Assert.AreEqual(Term.Constant(typeof(bool)), higherOrder.Body);
        }

        [Test]
        public void LambdaVariableBodyTest()
        {
            // a -> a
            var term =
                Term.Lambda(
                    "a",
                    Term.Identity("a"));

            var environment = Environment.Create();
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
                Term.Apply(
                    Term.Lambda(
                        "a",
                        Term.Identity("a")),
                    Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            // (a:bool -> a:bool) false:bool
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindConstant(bool result)
        {
            var term =
                Term.Bind(
                    "a",
                    Term.Constant(result),
                    Term.Not(
                        Term.Identity("a")));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void BindAppliedIdentity(bool result)
        {
            var term =
                Term.Apply(
                    Term.Lambda(
                        "b",
                        Term.Bind(
                            "a",
                            Term.Identity("b"),
                            Term.Not(
                                Term.Identity("a")))),
                    Term.Constant(result));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void BooleanAndBodyTest()
        {
            // a && b
            var term =
                Term.AndAlso(
                    Term.Identity("a"),
                    Term.Identity("b"));

            var environment = Environment.Create();
            var actual = (AndAlsoTerm)environment.Infer(term);

            // (a:bool && b:bool):bool
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.Lhs.HigherOrder);
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.Rhs.HigherOrder);
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }

        [Test]
        public void DoubleLambdaInsideBooleanAndBodyTest()
        {
            // a -> b -> a && b
            var term =
                Term.Lambda(
                    "a",
                    Term.Lambda(
                        "b",
                        Term.AndAlso(
                            Term.Identity("a"),
                            Term.Identity("b"))));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            var lambda = (LambdaTerm)actual;
            var higherOrder = (LambdaTerm)lambda.HigherOrder;

            // a:bool -> (b:bool -> (a:bool && b:bool):bool):bool
            Assert.AreEqual(Term.Constant(typeof(bool)), higherOrder.Parameter);
            Assert.AreEqual(Term.Constant(typeof(bool)), ((LambdaTerm)higherOrder.Body).Parameter);
            Assert.AreEqual(Term.Constant(typeof(bool)), ((LambdaTerm)higherOrder.Body).Body);
        }

        [Test]
        public void PassingLambdaArgument()
        {
            // (a -> b -> a b) (a -> a) false
            var term =
                Term.Apply(
                    Term.Apply(
                        Term.Lambda(
                            "a",
                            Term.Lambda(
                                "b",
                                Term.Apply(
                                    Term.Identity("a"),
                                    Term.Identity("b")))),
                        Term.Lambda(
                            "a",
                            Term.Identity("a"))),
                    Term.Constant(false));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            // (a:(bool -> bool) -> b:bool -> a:(bool -> bool) b:bool) (a:bool -> a:bool):(bool -> bool) false:bool
            Assert.AreEqual(Term.Constant(typeof(bool)), actual.HigherOrder);
        }
    }
}
