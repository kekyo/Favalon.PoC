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
    }
}
