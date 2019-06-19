using NUnit.Framework;

using Favalon.Expressions;

namespace Favalon
{
    using static Factory;

    [TestFixture]
    public sealed class InferenceTest
    {
        [Test]
        public void FromInteger()
        {
            var environment = new ExpressionEnvironment();

            var expression = Integer(123);

            var actual = expression.Infer(environment);

            Assert.AreEqual("123", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable()
        {
            var environment = new ExpressionEnvironment();

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
        }

        [Test]
        public void BindInteger()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Integer(123), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindPlaceholder()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Integer(123), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in y", actual.ReadableString);
            Assert.AreEqual("'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply("y", Integer(123)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'c", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply(Apply("y", Integer(123)), Integer(456)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 456 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'c", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply("y", Apply("z", Integer(456))), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y (z 456) in y", actual.ReadableString);
            Assert.AreEqual("'b -> System.Int32 -> 'c", actual.HigherOrder.ReadableString);
        }
    }
}
