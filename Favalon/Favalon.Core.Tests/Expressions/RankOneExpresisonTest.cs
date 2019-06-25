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
            var environment = new Environment();

            // System.Int32
            var expression = Type("System.Int32");

            var actual = (TypeExpression)expression.Infer(environment);

            Assert.AreEqual("System.Int32", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);
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
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void FromVariableWithAnnotation()
        {
            var environment = new Environment();

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
            var environment = new Environment();

            // x System.Int32
            var expression = Apply("x", Type("System.Int32"));

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyAnnotatedVariableAndType()
        {
            var environment = new Environment();

            // x:(* -> 'a) System.Int32
            var expression = Apply(Variable("x", Lambda(Kind(), environment.FreeVariable())), Type("System.Int32"));

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyVariableAndAnnotatedVariable()
        {
            var environment = new Environment();

            // x y:*
            var expression = Apply("x", Variable("y", Kind()));

            var actual = (ApplyExpression)expression.Infer(environment);

            Assert.AreEqual("x y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
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

            Assert.AreEqual("* -> 'a", actual.Function.HigherOrder.ReadableString);
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
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFreeVariable()
        {
            var environment = new Environment();

            // x = System.Int32 in y
            var expression = Bind("x", Type("System.Int32"), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in y", actual.ReadableString);
            Assert.AreEqual("'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable1()
        {
            var environment = new Environment();

            // x = y:* in x
            var expression = Bind("x", Variable("y", Kind()), "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindAnnotatedVariable2()
        {
            var environment = new Environment();

            // x:* = y in x
            var expression = Bind(Variable("x", Kind()), "y", "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction1()
        {
            var environment = new Environment();

            // x = y System.Int32 in y
            var expression = Bind("x", Apply("y", Type("System.Int32")), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 in y", actual.ReadableString);
            Assert.AreEqual("* -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindFunction2()
        {
            var environment = new Environment();

            // x = y System.Int32 System.Int64 in y
            var expression = Bind("x", Apply(Apply("y", Type("System.Int32")), Type("System.Int64")), "y");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = y System.Int32 System.Int64 in y", actual.ReadableString);
            Assert.AreEqual("* -> * -> 'a", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'T2", actual.Bound.HigherOrder.StrictReadableString);
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

            Assert.AreEqual("'T3", actual.Bound.HigherOrder.StrictReadableString);
        }

        [Test]
        public void BindShadowed1()
        {
            var environment = new Environment();

            // x = System.Int32 in x = y -> x in x
            var expression = Bind("x", Type("System.Int32"), Bind("x", Lambda("y", "x"), "x"));

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x = y -> x in x", actual.ReadableString);
            Assert.AreEqual("'a -> *", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
        }

        [Test]
        public void BindShadowed2()
        {
            var environment = new Environment();

            // x = y System.Int64
            environment.SetNamedExpression("x", Lambda("y", Type("System.Int64")).Infer(environment));

            // x = System.Int32 in x
            var expression = Bind("x", Type("System.Int32"), "x");

            var actual = (BindExpression)expression.Infer(environment);

            Assert.AreEqual("x = System.Int32 in x", actual.ReadableString);
            Assert.AreEqual("*", actual.HigherOrder.ReadableString);

            Assert.AreEqual("*", actual.Bound.HigherOrder.ReadableString);
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
            Assert.AreEqual("(* -> 'a) -> 'a", actual.HigherOrder.ReadableString);
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

        [Test]
        public void LambdaShadowed1()
        {
            var environment = new Environment();

            // x -> y -> x -> x System.Int32
            var expression = Lambda("x", Lambda("y", Lambda("x", Apply("x", Type("System.Int32")))));

            var actual = (LambdaExpression)expression.Infer(environment);

            Assert.AreEqual("x -> y -> x -> x System.Int32", actual.ReadableString);
            Assert.AreEqual("'a -> 'b -> (* -> 'c) -> 'c", actual.HigherOrder.ReadableString);

            Assert.AreEqual("'a", actual.Parameter.HigherOrder.ReadableString);
        }
    }
}
