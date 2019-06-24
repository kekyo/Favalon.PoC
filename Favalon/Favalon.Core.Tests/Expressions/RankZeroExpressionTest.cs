using NUnit.Framework;

namespace Favalon.Expressions
{
    using static Internals.StaticFactory;

    [TestFixture]
    public sealed class RankZeroExpressionTest
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromInteger()
        {
            var environment = new Environment();

            // 123
            var expression = Integer(123);

            var actual = expression.Infer(environment);

            Assert.AreEqual("123", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromVariable1()
        {
            var environment = new Environment();

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = new Environment();

            environment.SetNamedExpression("x", Integer(123));

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

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
        public void BindFunction1()
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
            Assert.AreEqual("System.Int32 -> System.Int32 -> 'a", actual.HigherOrder.ReadableString);
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
        }

        [Test]
        public void BindShadowed1()
        {
            var environment = new Environment();

            // x = 123 in x = y -> x in x
            var expression = Bind("x", Integer(123), Bind("x", Lambda("y", "x"), "x"));

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = 123 in x = y -> x in x", actual.ReadableString);
            Assert.AreEqual("'a -> System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed2()
        {
            var environment = new Environment();

            // x = z 456
            environment.SetNamedExpression("x", Lambda("z", Integer(456)).Infer(environment));

            // x = 123 in y
            var expression = Bind("x", Integer(123), "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Variable.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction1()
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
            Assert.AreEqual("('a -> 'b) -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction3()
        {
            var environment = new Environment();

            // x -> y -> x y
            var expression = Lambda("x", Lambda("y", Apply("x", "y")));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaShadowed1()
        {
            var environment = new Environment();

            // x -> y -> x -> x 123
            var expression = Lambda("x", Lambda("y", Lambda("x", Apply("x", Integer(123)))));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y -> x -> x 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> (System.Int32 -> 'c) -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Parameter.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        //[Test]
        //public void RecursiveFunction()
        //{
        //    var environment = new Environment();

        //    // x = y -> x 123 in x 456
        //    var expression = Bind("x", Lambda("y", Apply("x", Integer(123))), Apply("x", Integer(456)));

        //    var actual = (BindExpression)expression.Infer(environment);

        //    Assert.AreEqual("x -> y 123", actual.ReadableString);
        //    Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        //}
    }
}
