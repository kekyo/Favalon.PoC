using LambdaCalculus.Operators;
using NUnit.Framework;

namespace LambdaCalculus
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
            Assert.AreEqual(Term.Type<bool>(), actual.HigherOrder);
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
            Assert.AreEqual(Term.Type<bool>(), higherOrder.Body);
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
            Assert.AreEqual(Term.Type<bool>(), actual.HigherOrder);
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
            var actual = (Operators.AndAlsoTerm)environment.Infer(term);

            // (a:bool && b:bool):bool
            Assert.AreEqual(Term.Type<bool>(), actual.Lhs.HigherOrder);
            Assert.AreEqual(Term.Type<bool>(), actual.Rhs.HigherOrder);
            Assert.AreEqual(Term.Type<bool>(), actual.HigherOrder);
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
            Assert.AreEqual(Term.Type<bool>(), higherOrder.Parameter);
            Assert.AreEqual(Term.Type<bool>(), ((LambdaTerm)higherOrder.Body).Parameter);
            Assert.AreEqual(Term.Type<bool>(), ((LambdaTerm)higherOrder.Body).Body);
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
            Assert.AreEqual(Term.Type<bool>(), actual.HigherOrder);
        }

        [TestCase(true, 123, "abc")]
        [TestCase(false, 123, "abc")]
        public void IfFixedInferrable(bool condition, object then, object @else)
        {
            var term =
                Term.If(
                    Term.Constant(condition),
                    Term.Constant(then),
                    Term.Constant(@else));

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(
                condition ? Term.Constant(then).HigherOrder : Term.Constant(@else).HigherOrder,
                actual.HigherOrder);
        }

        [Test]
        public void IfCondition()
        {
            var term =
                Term.If(
                    Term.Identity("cond"),
                    Term.Constant(123),
                    Term.Constant("abc"));

            var environment = Environment.Create();
            var actual = (IfTerm)environment.Infer(term);

            Assert.AreEqual(
                Term.Type<bool>(),
                actual.Condition.HigherOrder);
        }

        [Test]
        public void IfAppliedCondition()
        {
            var term =
                Term.Lambda(
                    "cond",
                    Term.If(
                        Term.Identity("cond"),
                        Term.Constant(123),
                        Term.Constant("abc")));

            var environment = Environment.Create();
            var actual = (LambdaTerm)environment.Infer(term);

            Assert.AreEqual(Term.Type<bool>(), actual.Parameter.HigherOrder);
        }
    }
}
