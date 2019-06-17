using NUnit.Framework;

using Favalon.Expressions;

namespace Favalon
{
    using static Expression;

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

            var resolved = actual as IResolvedExpression;
            Assert.AreEqual("System.Int32", resolved?.HigherOrderExpression.ReadableString);
        }

        [Test]
        public void FromVariable()
        {
            var environment = new ExpressionEnvironment();

            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);

            var resolved = actual as IResolvedExpression;
            Assert.IsNull(resolved);
        }

        [Test]
        public void ApplyVariableAndInteger()
        {
            var environment = new ExpressionEnvironment();

            var expression = Apply(Variable("x"), Integer(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);

            var resolved = actual as IResolvedExpression;
            Assert.AreEqual("'a -> System.Int32", resolved?.HigherOrderExpression.ReadableString);
        }

        [Test]
        public void BindInteger()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind(Variable("x"), Integer(123), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);

            var resolved = actual as IResolvedExpression;
            Assert.AreEqual("System.Int32", resolved?.HigherOrderExpression.ReadableString);
        }
    }
}
