﻿using NUnit.Framework;

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
            var environment = ExpressionEnvironment.Create();

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
            var environment = ExpressionEnvironment.Create();

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = ExpressionEnvironment.Create();

            environment.Bind("x", Constant(123));

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariableWithAnnotation()
        {
            var environment = ExpressionEnvironment.Create();

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
            var environment = ExpressionEnvironment.Create();

            // x 123
            var expression = Apply("x", Constant(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x 123", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyAnnotatedVariableAndInteger()
        {
            var environment = ExpressionEnvironment.Create();

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
            var environment = ExpressionEnvironment.Create();

            // x y:System.Int32
            var expression = Apply("x", Variable("y", Type("System.Int32")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredIntegerExpression()
        {
            var environment = ExpressionEnvironment.Create();

            environment.Bind("v", Constant(123));

            // x v
            var expression = Apply("x", "v");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void BindInteger()
        {
            var environment = ExpressionEnvironment.Create();

            // x = 123 in x
            var expression = Bind("x", Constant(123), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFreeVariable()
        {
            var environment = ExpressionEnvironment.Create();

            // x = 123 in y
            var expression = Bind("x", Constant(123), "y");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable1()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y:System.Int32 in x
            var expression = Bind("x", Variable("y", Type("System.Int32")), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable2()
        {
            var environment = ExpressionEnvironment.Create();

            // x:System.Int32 = y in x
            var expression = Bind(Variable("x", Type("System.Int32")), "y", "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedEnvironment1()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y:System.Int32
            environment.Bind("x", Variable("y", Type("System.Int32")));

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedEnvironment2()
        {
            var environment = ExpressionEnvironment.Create();

            // x:System.Int32 = y
            environment.Bind(Variable("x", Type("System.Int32")), "y");

            // x
            var expression = Variable("x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction1()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y 123 in x
            var expression = Bind("x", Apply("y", Constant(123)), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> 'a", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y 123 456 in y
            var expression = Bind("x", Apply(Apply("y", Constant(123)), Constant(456)), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y 123 456 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32 -> System.Int32 -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Function).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y (z 123) in x
            var expression = Bind("x", Apply("y", Apply("z", Constant(123))), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y (z 123) in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a -> 'b", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
            Assert.AreEqual("System.Int32 -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Argument).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed1()
        {
            var environment = ExpressionEnvironment.Create();

            // x = 123 in x = y -> x in x
            var expression = Bind("x", Constant(123), Bind("x", Lambda("y", "x"), "x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x = y -> x in x", actual.ReadableString);
            Assert.AreEqual("'a -> System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed2()
        {
            var environment = ExpressionEnvironment.Create();

            // x = y 456
            environment.Bind("x", Lambda("y", Constant(456)));

            // x = 123 in x
            var expression = Bind("x", Constant(123), "x");

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = 123 in x", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);

            Assert.AreEqual("System.Int32", actual.Bound.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction1()
        {
            var environment = ExpressionEnvironment.Create();

            // x -> y 123
            var expression = Lambda("x", Apply("y", Constant(123)));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = ExpressionEnvironment.Create();

            // x -> x y
            var expression = Lambda("x", Apply("x", "y"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction3()
        {
            var environment = ExpressionEnvironment.Create();

            // x -> y -> x y
            var expression = Lambda("x", Lambda("y", Apply("x", "y")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaShadowed1()
        {
            var environment = ExpressionEnvironment.Create();

            // x -> y -> x -> x 123
            var expression = Lambda("x", Lambda("y", Lambda("x", Apply("x", Constant(123)))));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x -> x 123", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> (System.Int32 -> 'c) -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Parameter.HigherOrder.ReadableString);
        }
    }
}
