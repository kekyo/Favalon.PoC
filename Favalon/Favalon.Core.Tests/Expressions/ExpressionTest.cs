using NUnit.Framework;

namespace Favalon.Expressions
{
    using static Internals.StaticFactory;

    [TestFixture]
    public sealed class ExpressionTest
    {
        [Test]
        public void MethodCall()
        {
            var environment = Environment.Create();

            // System.Int32.Parse = System.String -> System.Int32
            environment.Register(Variable("System.Int32.Parse", Lambda(Type("System.String"), Type("System.Int32"))));

            // int = System.Int32.Parse
            environment.Bind(Variable("int"), Variable("System.Int32.Parse"));

            // int "123"
            var expression = Apply(Variable("int"), Constant("123"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("int \"123\"", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void New1()
        {
            var environment = Environment.Create();

            // new System.Collections.ArrayList ()
            var expression = Apply(New(Type("System.Collections.ArrayList")), Constant(123));

            var actual = expression.Infer(environment);

            Assert.AreEqual("new System.Collections.ArrayList 123", actual.ReadableString);
            Assert.AreEqual("System.Collections.ArrayList", actual.HigherOrder.ReadableString);
        }

        //[Test]
        //public void NewWithCompositionedType()
        //{
        //    var environment = Environment.Create();
        //    environment.Register(Variable("new", Lambda(Kind(), environment.Placeholder())));
        //    environment.Register(Variable("list", Lambda(Kind(), Kind())));

        //    // new (list System.Int32)
        //    var expression = Apply(Variable("new"), Apply(Variable("list"), Type("System.Int32")));

        //    var actual = expression.Infer(environment);

        //    Assert.AreEqual("list System.Int32", actual.ReadableString);
        //    Assert.AreEqual("*", actual.HigherOrder.ReadableString);
        //}
    }
}
