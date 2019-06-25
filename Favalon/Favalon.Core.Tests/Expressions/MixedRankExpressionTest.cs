using NUnit.Framework;

namespace Favalon.Expressions
{
    using static Internals.StaticFactory;

    [TestFixture]
    public sealed class MixedRankExpressionTest
    {
        [Test]
        public void Generator1()
        {
            var environment = ExpressionEnvironment.Create();

            // 123 <-- int("123")
            // int = a -> System.Int32.Parse a
            // environment.Bind("int", Lambda("a", Call("System.Int32.Parse", "a")));
            environment.Bind("int", Variable("System.Int32.Parse", Lambda(Type("System.String"), Type("System.Int32"))));

            // Expression Call(VariableExpression function, Expression argument);

            // int "123"
            var expression = Apply("int", Constant("123"));

            var actual = expression.Infer(environment);

            Assert.AreEqual("int \"123\"", actual.ReadableString);
            Assert.AreEqual("System.Int32", actual.HigherOrder.ReadableString);
        }
    }
}
