using NUnit.Framework;

namespace Favalon.Expressions
{
    using static Internals.StaticFactory;

    [TestFixture]
    public sealed class RankOneExpressionTest
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromType()
        {
            var environment = Environment.Create();

            // System.Int32
            var expression = Type("System.Int32");

            var actual = expression.Infer(environment);

            Assert.AreEqual("System.Int32", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void FromVariable1()
        {
            var environment = Environment.Create();

            // TFoo
            var expression = Variable("TFoo");

            var actual = expression.Infer(environment);

            Assert.AreEqual("TFoo", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariable2()
        {
            var environment = Environment.Create();

            environment.Bind("TFoo", Type("TBar"));

            // TFoo
            var expression = Variable("TFoo");

            var actual = expression.Infer(environment);

            Assert.AreEqual("TFoo", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariableWithAnnotation()
        {
            var environment = Environment.Create();

            // x:*
            var expression = Variable("x", Kind());

            var actual = expression.Infer(environment);

            Assert.AreEqual("x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ApplyVariableAndType()
        {
            var environment = Environment.Create();

            // x System.Int32
            var expression = Apply(Variable("x"), Type("System.Int32"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyAnnotatedVariableAndType()
        {
            var environment = Environment.Create();

            // x:(* -> 'a) System.Int32
            var expression = Apply(Variable("x", Lambda(Kind(), environment.FreeVariable())), Type("System.Int32"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyVariableAndAnnotatedVariable()
        {
            var environment = Environment.Create();

            // x y:*
            var expression = Apply(Variable("x"), Variable("y", Kind()));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyRegisteredTypeExpression()
        {
            var environment = Environment.Create();

            environment.Bind("v", Type("System.Int32"));

            // x v
            var expression = Apply(Variable("x"), Variable("v"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x v", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void BindInteger()
        {
            var environment = Environment.Create();

            // x = System.Int32 in x
            var expression = Bind(Variable("x"), Type("System.Int32"), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFreeVariable()
        {
            var environment = Environment.Create();

            // x = System.Int32 in y
            var expression = Bind(Variable("x"), Type("System.Int32"), Variable("y"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable1()
        {
            var environment = Environment.Create();

            // x = y:* in x
            var expression = Bind(Variable("x"), Variable("y", Kind()), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable2()
        {
            var environment = Environment.Create();

            // x:* = y in x
            var expression = Bind(Variable("x", Kind()), Variable("y"), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction1()
        {
            var environment = Environment.Create();

            // x = y System.Int32 in x
            var expression = Bind(Variable("x"), Apply(Variable("y"), Type("System.Int32")), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = Environment.Create();

            // x = y System.Int32 System.Int64 in x
            var expression = Bind(Variable("x"), Apply(Apply(Variable("y"), Type("System.Int32")), Type("System.Int64")), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 System.Int64 in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> * -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Function).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction3()
        {
            var environment = Environment.Create();

            // x = y (z System.Int32) in x
            var expression = Bind(Variable("x"), Apply(Variable("y"), Apply(Variable("z"), Type("System.Int32"))), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = y (z System.Int32) in x", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a -> 'b", ((ApplyExpression)actual.Expression).Function.HigherOrder.ReadableString);
            Assert.AreEqual("* -> 'a", ((ApplyExpression)((ApplyExpression)actual.Expression).Argument).Function.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed1()
        {
            var environment = Environment.Create();

            // x = System.Int32 in x = y -> x in x
            var expression = Bind(Variable("x"), Type("System.Int32"), Bind(Variable("x"), Lambda(Variable("y"), Variable("x")), Variable("x")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x = y -> x in x", actual.ReadableString);
            Assert.AreEqual("'a -> *", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed2()
        {
            var environment = Environment.Create();

            // x = y System.Int64
            environment.Bind("x", Lambda(Variable("y"), Type("System.Int64")).Infer(environment));

            // x = System.Int32 in x
            var expression = Bind(Variable("x"), Type("System.Int32"), Variable("x"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////

        [Test]
        public void LambdaFunction1()
        {
            var environment = Environment.Create();

            // x -> y System.Int32
            var expression = Lambda(Variable("x"), Apply(Variable("y"), Type("System.Int32")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y System.Int32", actual.ReadableString);
            Assert.AreEqual("'a -> 'b", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void LambdaFunction2()
        {
            var environment = Environment.Create();

            environment.Bind("y", Type("TY"));

            // x -> x y
            var expression = Lambda(Variable("x"), Apply(Variable("x"), Variable("y")));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> x y", actual.ReadableString);
            Assert.AreEqual("(* -> 'a) -> 'a", actual.HigherOrder.ReadableString);
        }

#if false
        [Test]
        public void LambdaFunction3()
        {
            var environment = ExpressionEnvironment.Create();

            // x -> y -> x y
            var expression = Lambda("x", Lambda("y", Apply("x", "y")));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y -> x y", actual.ReadableString);
            Assert.AreEqual("('a -> 'b) -> 'a -> 'b", actual.HigherOrder.ReadableString);
        }
#endif

        [Test]
        public void LambdaShadowed1()
        {
            var environment = Environment.Create();

            // x -> y -> x -> x System.Int32
            var expression = Lambda(Variable("x"), Lambda(Variable("y"), Lambda(Variable("x"), Apply(Variable("x"), Type("System.Int32")))));

            var actual = expression.Infer(environment);

            Assert.AreEqual("x -> y -> x -> x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> (* -> 'c) -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Parameter.HigherOrder.ReadableString);
        }
    }
}
