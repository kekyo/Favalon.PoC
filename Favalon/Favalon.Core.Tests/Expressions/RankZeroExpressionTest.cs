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
            var environment = Environment.Create();

            // 123
            var expression = Constant(123);

            var actual = expression.Infer(environment);

            Assert.AreEqual("123", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromVariable1()
        {
            var environment = Environment.Create();

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = Environment.Create();

            environment.Bind(Variable("x"), Constant(123));

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariableWithAnnotation()
        {
            var environment = Environment.Create();

            // x:System.Int32
            var expression = Variable("x", Type("System.Int32"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ApplyVariableAndInteger()
        {
            var environment = Environment.Create();

            // x 123
            var expression = Apply(Variable("x"), Constant(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyAnnotatedVariableAndInteger()
        {
            var environment = Environment.Create();

            // x:(System.Int32 -> 'a) 123
            var expression = Apply(Variable("x", Lambda(Type("System.Int32"), environment.FreeVariable())), Constant(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyVariableAndAnnotatedVariable()
        {
            var environment = Environment.Create();

            // x y:System.Int32
            var expression = Apply(Variable("x"), Variable("y", Type("System.Int32")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredIntegerExpression()
        {
            var environment = Environment.Create();

            environment.Bind(Variable("v"), Constant(123));

            // x v
            var expression = Apply(Variable("x"), Variable("v"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void BindInteger()
        {
            var environment = Environment.Create();

            // x = 123 in x
            var expression = Bind(Variable("x"), Constant(123), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFreeVariable()
        {
            var environment = Environment.Create();

            // x = 123 in y
            var expression = Bind(Variable("x"), Constant(123), Variable("y", environment.FreeVariable()));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable1()
        {
            var environment = Environment.Create();

            // x = y:System.Int32 in x
            var expression = Bind(Variable("x"), Variable("y", Type("System.Int32")), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable2()
        {
            var environment = Environment.Create();

            // x:System.Int32 = y in x
            var expression = Bind(Variable("x", Type("System.Int32")), Variable("y"), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction1()
        {
            var environment = Environment.Create();

            // x = y 123 in x
            var expression = Bind(Variable("x"), Apply(Variable("y"), Constant(123)), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = Environment.Create();

            // x = y 123 456 in y
            var expression = Bind(Variable("x"), Apply(Apply(Variable("y"), Constant(123)), Constant(456)), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 456 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> System.Int32 -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Function).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = Environment.Create();

            // x = y (z 123) in x
            var expression = Bind(Variable("x"), Apply(Variable("y"), Apply(Variable("z"), Constant(123))), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y (z 123) in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a -> 'b", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
            Assert.AreEqual("System.Int32 -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Argument).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed1()
        {
            var environment = Environment.Create();

            // x = 123 in x = y -> x in x
            var expression = Bind(Variable("x"), Constant(123), Bind(Variable("x"), Lambda(Variable("y"), Variable("x")), Variable("x")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x = y -> x in x", actual.ReadableString);
            Assert.AreEqual("'a -> System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed2()
        {
            var environment = Environment.Create();

            // x = y 456
            environment.Bind(Variable("x"), Lambda(Variable("y"), Constant(456)));

            // x = 123 in x
            var expression = Bind(Variable("x"), Constant(123), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction1()
        {
            var environment = Environment.Create();

            // x -> y 123
            var expression = Lambda(Variable("x"), Apply(Variable("y"), Constant(123)));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = Environment.Create();

            // x -> x y
            var expression = Lambda(Variable("x"), Apply(Variable("x"), Variable("y")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction3()
        {
            var environment = Environment.Create();

            // x -> y -> x y
            var expression = Lambda(Variable("x"), Lambda(Variable("y"), Apply(Variable("x"), Variable("y"))));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaShadowed1()
        {
            var environment = Environment.Create();

            // x -> y -> x -> x 123
            var expression = Lambda(Variable("x"), Lambda(Variable("y"), Lambda(Variable("x"), Apply(Variable("x"), Constant(123)))));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x -> x 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> (System.Int32 -> 'c) -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Parameter.HigherOrder.ReadableString);
        }
    }
}
