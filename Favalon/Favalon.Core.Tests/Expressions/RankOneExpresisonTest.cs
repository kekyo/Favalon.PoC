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
        public void FromVariable1()
        {
            var environment = new Environment();

            // TFoo
            var expression = Variable("TFoo");

            var actual = expression.Infer(environment);

            Assert.AreEqual("TFoo", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = new Environment();

            environment.SetNamedExpression("TFoo", Type("TBar"));

            // TFoo
            var expression = Variable("TFoo");

            var actual = expression.Infer(environment);

            Assert.AreEqual("TFoo", actual.ReadableString);
            Assert.AreEqual("(Kind)", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ApplyVariableAndType()
        {
            var environment = new Environment();

            // x System.Int32
            var expression = Apply("x", Type("System.Int32"));

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("(Kind) -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredTypeExpression()
        {
            var environment = new Environment();
            environment.SetNamedExpression("v", Type("System.Int32"));

            // x v
            var expression = Apply("x", "v");

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("(Kind) -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void BindInteger()
        {
            var environment = new Environment();

            // x = System.Int32 in x
            var expression = Bind("x", Type("System.Int32"), "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x", actual.ReadableString);
            Assert.AreEqual("(Kind)", actual.HigherOrder.ReadableString);

            Assert.AreEqual("(Kind)", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindPlaceholder()
        {
            var environment = new Environment();

            // x = System.Int32 in y
            var expression = Bind("x", Type("System.Int32"), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("(Kind)", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction1()
        {
            var environment = new Environment();

            // x = y System.Int32 in y
            var expression = Bind("x", Apply("y", Type("System.Int32")), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 in y", actual.ReadableString);
            Assert.AreEqual("(Kind) -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new Environment();

            // x = y System.Int32 System.Int64 in y
            var expression = Bind("x", Apply(Apply("y", Type("System.Int32")), Type("System.Int64")), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 System.Int64 in y", actual.ReadableString);
            Assert.AreEqual("(Kind) -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'b", actual.Variable.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = new Environment();

            // x = y (z System.Int32) in y
            var expression = Bind("x", Apply("y", Apply("z", Type("System.Int32"))), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y (z System.Int32) in y", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'b", actual.Variable.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction1()
        {
            var environment = new Environment();

            // x -> y System.Int32
            var expression = Lambda("x", Apply("y", Type("System.Int32")));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y System.Int32", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = new Environment();

            environment.SetNamedExpression("y", Type("TY"));

            // x -> x y
            var expression = Lambda("x", Apply("x", "y"));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("((Kind) -> 'a) -> 'a", actual.HigherOrder.ReadableString);
        }

#if false
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
#endif
    }
}
