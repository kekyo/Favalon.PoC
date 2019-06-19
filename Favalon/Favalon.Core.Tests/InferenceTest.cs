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

            Assert.AreEqual("x:'a", actual.ReadableString);
        }

        [Test]
        public void ApplyVariableAndInteger()
        {
            var environment = new ExpressionEnvironment();

            // x 123
            var expression = Apply("x", Integer(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:(System.Int32 -> 'b) 123):'b", actual.ReadableString);
        }

        [Test]
        public void BindInteger()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Integer(123), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:System.Int32 = 123 in x:System.Int32):System.Int32", actual.ReadableString);
        }

        [Test]
        public void BindPlaceholder()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Integer(123), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:System.Int32 = 123 in y:'b):'b", actual.ReadableString);
        }

        [Test]
        public void BindFunction()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply("y", Integer(123)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:'c = (y:(System.Int32 -> 'c) 123):'c in y:(System.Int32 -> 'c)):(System.Int32 -> 'c)", actual.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply(Apply("y", Integer(123)), Integer(456)), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:'d = ((y:(System.Int32 -> 'c) 123):(System.Int32 -> 'd) 456):'d in y:(System.Int32 -> 'c)):(System.Int32 -> 'c)", actual.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = new ExpressionEnvironment();

            var expression = Bind("x", Apply("y", Apply("z", Integer(456))), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("(x:'e = (y:('d -> 'e) ((z:(System.Int32 -> 'd) 456):'d)):'e in y:('d -> 'e)):('d -> 'e)", actual.ReadableString);
        }
    }
}
