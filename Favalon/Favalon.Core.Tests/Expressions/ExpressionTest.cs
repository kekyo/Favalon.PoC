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

            var actual = environment.Infer(expression);

            Assert.AreEqual("int \"123\"", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyNewAndType()
        {
            var environment = Environment.Create();

            // new System.Collections.ArrayList ()
            var expression = Apply(New(Type("System.Collections.ArrayList")), Constant(123));

            var actual = environment.Infer(expression);

            Assert.AreEqual("new System.Collections.ArrayList 123", actual.ReadableString);
            Assert.AreEqual("System.Collections.ArrayList", actual.HigherOrder.ReadableString);
        }

        [Test]
        public void ApplyWithNewOperator()
        {
            var environment = Environment.Create();
            //environment.Bind(Variable("new"), New(environment.Placeholder(Kind())));
            environment.Bind(Variable("new"), Lambda(Variable("ty"), New(Variable("ty"))));

            // new System.Collections.ArrayList ()
            var expression = Apply(Apply(Variable("new"), Type("System.Collections.ArrayList")), Constant(123));
            //var expression = Apply(Variable("new"), Type("System.Collections.ArrayList"));

            var actual = environment.Infer(expression);

            Assert.AreEqual("new System.Collections.ArrayList 123", actual.ReadableString);
            Assert.AreEqual("System.Collections.ArrayList", actual.HigherOrder.ReadableString);
        }
    }
}
