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
    }
}
