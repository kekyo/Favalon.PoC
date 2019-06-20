using NUnit.Framework;

using Favalon.Expressions;

namespace Favalon
{
    using static StaticFactory;

    [TestFixture]
    public sealed class InferenceTest
    {
        [Test]
        public void FromInteger()
        {
            var environment = new ExpressionEnvironment();

            // 123
            var expression = Integer(123);

            var actual = expression.Infer(environment);

            Assert.AreEqual("123", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable()
        {
            var environment = new ExpressionEnvironment();

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyVariableAndInteger()
        {
            var environment = new ExpressionEnvironment();

            // x 123
            var expression = Apply("x", Integer(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);
            Assert.AreEqual("'b", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'b", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindInteger()
        {
            var environment = new ExpressionEnvironment();

            // x = 123 in x
            var expression = Bind("x", Integer(123), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindPlaceholder()
        {
            var environment = new ExpressionEnvironment();

            // x = 123 in y
            var expression = Bind("x", Integer(123), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in y", actual.ReadableString);
            Assert.AreEqual("'b", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction()
        {
            var environment = new ExpressionEnvironment();

            // x = y 123 in y
            var expression = Bind("x", Apply("y", Integer(123)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'c", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new ExpressionEnvironment();

            // x = y 123 456 in y
            var expression = Bind("x", Apply(Apply("y", Integer(123)), Integer(456)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 456 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'d", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = new ExpressionEnvironment();

            // x = y (z 123) in y
            var expression = Bind("x", Apply("y", Apply("z", Integer(123))), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y (z 123) in y", actual.ReadableString);
            Assert.AreEqual("'d -> 'e", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'e", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction()
        {
            var environment = new ExpressionEnvironment();

            // x -> y 123
            var expression = Lambda("x", Apply("y", Integer(123)));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'c", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = new ExpressionEnvironment();

            // x -> x y
            var expression = Lambda("x", Apply("x", "y"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("'a -> 'c", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction3()
        {
            var environment = new ExpressionEnvironment();

            // x -> y -> x y
            var expression = Lambda("x", Lambda("y", Apply("x", "y")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("'b -> 'c -> 'c", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredIntegerExpression()
        {
            var environment = new ExpressionEnvironment();
            environment.SetNamedExpression("v", Integer(123));

            // x v
            var expression = Apply("x", "v");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'b", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'b", actual.Function.HigherOrder.ReadableString);
        }
    }
}
