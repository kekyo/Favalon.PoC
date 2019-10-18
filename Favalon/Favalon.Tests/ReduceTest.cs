using Favalon.Expressions;
using NUnit.Framework;
using System;

namespace Favalon
{
    using static Favalon.Expressions.Factories;

    [TestFixture]
    public sealed class ReduceTest
    {
        [Test]
        public void DotNetMethod()
        {
            var parse = typeof(int).GetMethod("Parse", new[] { typeof(string) })!;
            var expression = CallMethod(parse, Value("123"));

            var actual = expression.Run();

            Assert.AreEqual(Value(123), actual);
        }

        [Test]
        public void ExecuteDirect()
        {
            var expression = RunExecutable(@"C:\Program Files\Git\usr\bin\wc.exe", Value("abcde fghij"));

            var actual = expression.Run();

            var splitted = ((string)(((Value)actual)!.RawValue!)).
                Split(
                    new[] { ' ', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);

            // Print newline, word, and byte counts 
            Assert.AreEqual(new[] { "0", "2", "11" }, splitted);
        }

        [Test]
        public void ExecuteBound()
        {
            var environment = Environment.Create();

            var expression = environment.Infer(
                Terms.Factories.Apply(
                    Terms.Factories.Variable("wc"),
                    Terms.Factories.String("abcde fghij")));

            var actual = expression.Run();

            var splitted = ((string)(((Value)actual)!.RawValue!)).
                Split(
                    new[] { ' ', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);

            // Print newline, word, and byte counts 
            Assert.AreEqual(new[] { "0", "2", "11" }, splitted);
        }
    }
}
