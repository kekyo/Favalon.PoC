using NUnit.Framework;

namespace Favalon.Expressions
{
    using static StaticFactory;

    [TestFixture]
    public sealed class RankOneExpressionTest
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromType()
        {
            var environment = new Environment();

            // System.Int32
            var expression = Type("System.Int32");

            var actual = (TypeExpression)expression.Infer(environment);

            Assert.AreEqual("System.Int32", actual.ReadableString);
            Assert.AreEqual("(Kind)", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromVariable()
        {
            var environment = new Environment();

            // 'T
            var expression = Variable("'T");

            var actual = expression.Infer(environment);

            Assert.AreEqual("'T", actual.ReadableString);
            Assert.AreEqual("(Kind)", actual.HigherOrder.ReadableString);
        }

#if false
        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ApplyVariableAndInteger()
        {
            var environment = new Environment();

            // x 123
            var expression = Apply("x", Integer(123));

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredIntegerExpression()
        {
            var environment = new Environment();
            environment.SetNamedExpression("v", Integer(123));

            // x v
            var expression = Apply("x", "v");

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void BindInteger()
        {
            var environment = new Environment();

            // x = 123 in x
            var expression = Bind("x", Integer(123), "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindPlaceholder()
        {
            var environment = new Environment();

            // x = 123 in y
            var expression = Bind("x", Integer(123), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = 123 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction()
        {
            var environment = new Environment();

            // x = y 123 in y
            var expression = Bind("x", Apply("y", Integer(123)), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y 123 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new Environment();

            // x = y 123 456 in y
            var expression = Bind("x", Apply(Apply("y", Integer(123)), Integer(456)), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y 123 456 in y", actual.ReadableString);
            Assert.AreEqual("System.Int32 -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'b", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = new Environment();

            // x = y (z 123) in y
            var expression = Bind("x", Apply("y", Apply("z", Integer(123))), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y (z 123) in y", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'b", actual.Variable.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction()
        {
            var environment = new Environment();

            // x -> y 123
            var expression = Lambda("x", Apply("y", Integer(123)));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = new Environment();

            // x -> x y
            var expression = Lambda("x", Apply("x", "y"));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction3()
        {
            var environment = new Environment();

            // x -> y -> x y
            var expression = Lambda("x", Lambda("y", Apply("x", "y")));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> 'a -> 'b", actual.HigherOrder.ReadableString);
        }
#endif
    }
}
